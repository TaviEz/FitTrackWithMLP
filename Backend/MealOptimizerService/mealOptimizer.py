import numpy as np
from scipy.optimize import minimize

from payloads.optimizedIngredientResponse import OptimizedIngredientResponse
from payloads.optimizitationResultResponse import OptimizationResultResponse

class MealOptimizer:
    def __init__(self, all_foods):
        self.db = {}
        for food in all_foods:
            # Standardize name and store 1g values
            self.db[food.name] = {
                "category": food.category.strip(),
                "values": np.array([
                    food.calories / 100.0,
                    food.protein / 100.0,
                    food.fat / 100.0,
                    food.carbs / 100.0
                ])
            }

    def optimize_meal(self, ingredients, targets):
        """The Linear Programming Solver for a single meal."""
        # Filter ingredients that exist in our DB
        valid_ingredients = [ing for ing in ingredients if ing.name in self.db]

        # DEBUG: See what was missed
        if len(valid_ingredients) < len(ingredients):
            missing = set(ingredients) - set(valid_ingredients)
            print(f"Missing from CSV: {missing.food_id} {missing.name}")
        if not valid_ingredients: return None

        # Matrix of nutrients: rows=ingredients, cols=[Cal, P, F, C]
        data_matrix = np.array([self.db[ing.name]["values"] for ing in valid_ingredients])
        
        # Objective: Square error of Calories
        def objective(weights):
            current_cal = np.dot(weights, data_matrix[:, 0])
            current_p   = np.dot(weights, data_matrix[:, 1])
            
            # Calculate the squared error for both
            cal_error = (current_cal - targets.calories)**2
            p_error = (current_p - targets.protein)**2
            
            # We multiply p_error because 1g of protein is 'worth' more 
            # to your goal than 1 single calorie.
            return cal_error + (p_error * 20)

        # Constraints
        unit_fat      = data_matrix[:, 2]

        def check_min_fat(w):
            total_f = np.dot(w, unit_fat)
            return total_f - targets.min_fat

        constraints = [
            {'type': 'ineq', 'fun': check_min_fat}
        ]

        # Bounds: (Min grams, Max grams) per ingredient
        # Initial guess: Start with a reasonable portion size for each ingredient category
        bounds = []
        initial_guess = []

        for ing in valid_ingredients:
            category = self.db[ing.name]["category"]

            if category == "Proteins":
                bounds.append((30, 300))
                initial_guess.append(100)

            elif category == "Dairy":
                if any(k in ing.name for k in ["Cheese", "Powder"]):
                    bounds.append((15, 100))
                    initial_guess.append(30)
                else: # Milk or Yogurt
                    bounds.append((30, 300))
                    initial_guess.append(150)

            elif category == "Carbs (Dry)":
                if "Honey" in ing.name:
                    bounds.append((5, 25))
                    initial_guess.append(10)
                else:
                    bounds.append((20, 300))
                    initial_guess.append(60)

            elif category == "Bread/Bakery":
                bounds.append((30, 150))
                initial_guess.append(40)

            elif category == "Fats":
                if "Olive Oil" in ing.name:
                    bounds.append((5, 20))
                else:
                    bounds.append((5, 80))
                initial_guess.append(15)
            
            elif category == "Sauces":
                bounds.append((5, 100))
                initial_guess.append(20)

            elif category == "Spreads":
                bounds.append((10, 100))
                initial_guess.append(20)

            elif category == "Veg/Fruit":
                bounds.append((30, 200))
                initial_guess.append(100)

            elif category == "Flavor/Aromatics":
                bounds.append((2, 20))
                initial_guess.append(10)
                
        res = minimize(objective, initial_guess, method='SLSQP', bounds=bounds, constraints=constraints)

        if res.success:
            optimized_ingredients = []
            for ing, weight in zip(valid_ingredients, res.x):
                optimized_ingredients.append(
                    OptimizedIngredientResponse(
                        food_id=ing.food_id,
                        name=ing.name,
                        amount_g=int(weight),
                        calories=int(ing.calories),
                        protein=ing.protein,
                        fats=ing.fats,
                        carbs=ing.carbs
                    )
                )

            return OptimizationResultResponse(
                ingredients=optimized_ingredients,
                error=res.fun
            )
        return None