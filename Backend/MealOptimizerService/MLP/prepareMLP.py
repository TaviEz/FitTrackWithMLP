import torch
import joblib
import numpy as np
from mealOptimizer import MealOptimizer
from MLP.mealSelectorMLP import MealSelectorMLP
from repositories.foodRepository import FoodRepository
from repositories.mealRepository import MealRepository

model_path = "./data/meal_selector_model.pth"
scaler_path = "./data/scaler.joblib"
id_mapping = joblib.load('./data/id_mapping.joblib')

foodRepository = FoodRepository()
mealRepository = MealRepository()
all_foods = foodRepository.get_all_foods()
optimizer = MealOptimizer(all_foods)

model = MealSelectorMLP(len(id_mapping))
model.load_state_dict(torch.load(model_path))
model.eval() # CRITICAL: Sets model to 'prediction' mode

scaler = joblib.load(scaler_path)

def predict_meals(calories, protein, meal_type_id, k=5):
    # Prepare input as a 2D array for the scaler
    raw_input = np.array([[calories, protein, meal_type_id]])
    
    # Scale the input
    scaled_input = scaler.transform(raw_input)
    input_tensor = torch.FloatTensor(scaled_input)

    # Prediction
    with torch.no_grad():
        logits = model(input_tensor)
        # Get the indices of the top K highest scores
        top_values, top_indices = torch.topk(logits, k, dim=1)
        
    return top_indices[0].tolist()

def generate_daily_plan(goals: dict, target_complexity: str = "Standard"):
    splits = {'BREAKFAST': 0.25, 'LUNCH': 0.30, 'SNACK': 0.15, 'DINNER': 0.30}
    meal_categories = ['BREAKFAST', 'LUNCH', 'SNACK', 'DINNER']
    daily_plan = []

    # Build Daly Plan
    for meal_type_id, category in enumerate(meal_categories):
        ratio = splits.get(category, 0.25)
        meal_targets = {
            'calories': goals['cal'] * ratio,
            'protein': goals['p'] * ratio,
            'min_fat': goals['f_min'] * ratio
        }

        # Get Top k suggestions from the model for this meal type
        k_value = 15 if target_complexity == "Simple" else 5
        top_indices = predict_meals(meal_targets['calories'], meal_targets['protein'], meal_type_id, k=k_value)

        suggested_ids = [id_mapping[p_idx] for p_idx in top_indices]
        
        # Iterate through suggestions to find the complexity match
        final_meal_data = None
        for actual_id in suggested_ids:
            meal_data = mealRepository.get_meal_details_by_id(actual_id)
            if meal_data and meal_data.complexity == target_complexity and meal_data.meal_type_id == meal_type_id:
                final_meal_data = meal_data
                break
        
        # Fallback: If no complexity match, take the model's #1 macro choice
        if not final_meal_data:
            final_meal_data = mealRepository.get_meal_details_by_id(id_mapping[top_indices[0]])

        optimization_result = optimizer.optimize_meal(final_meal_data.ingredients, meal_targets)    
        meal_entry = {
            "category": category,
            "title": final_meal_data.title,
            "meal_id": final_meal_data.id,
            "ingredients": [],
            "error": 0.0
        }

        if optimization_result:
            meal_entry["error"] = optimization_result["error"]
            for ing_name, weight in optimization_result["weights"].items():
                meal_entry["ingredients"].append({
                    "name": ing_name,
                    "amount_g": int(weight)
                })
        
        daily_plan.append(meal_entry)
    return daily_plan