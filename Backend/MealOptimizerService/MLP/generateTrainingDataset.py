import pandas as pd
import random
import sys
import os

# Get the path of the folder one level up (the root)
root_path = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
sys.path.insert(0, root_path)

# Now these will work perfectly
from repositories.foodRepository import FoodRepository
from repositories.mealRepository import MealRepository
from mealOptimizer import MealOptimizer

data = []
splits = [0.25, 0.3, 0.15, 0.3]
total_rows = 5000

for _ in range(total_rows):
    tdee = random.randint(1500, 4000)
    weight = random.randint(35, 150)
    protein_target = weight * random.choice([1.6, 1.8, 2.0, 2.2])
    meal_type_id = random.choice([0, 1, 2, 3])
    
    data.append({
        'calories': int(tdee * splits[meal_type_id]),
        'protein': int(protein_target * splits[meal_type_id]),
        'min_fat': 10,
        'meal_type_id': meal_type_id
    })

df_inputs = pd.DataFrame(data)

mealRepository = MealRepository()
foodRepository = FoodRepository()
all_foods = foodRepository.get_all_foods()
optimizer = MealOptimizer(all_foods)
all_recipes = mealRepository.get_all_meals_details()

meal_ids = []
 
for index, row in df_inputs.iterrows():
    best_meal_id = None
    min_error = float('inf')
    
    # Get all meals available for this MealTypeId
    eligible_meals = [m for m in all_recipes if m.meal_type_id == row['meal_type_id']]
    
    for meal in eligible_meals:
        opt_result = optimizer.optimize_meal(meal.ingredients, row)
        
        if opt_result and opt_result['error'] < min_error:
            min_error = opt_result['error']
            best_meal_id = meal.id

    meal_ids.append(best_meal_id)

    current_count = index + 1
    if current_count % 50 == 0:
        percentage = (current_count / total_rows) * 100
        print(f"Processed {current_count}/{total_rows} meals ({percentage:.1f}%)")

df_inputs['meal_id_suggestion'] = meal_ids

training_data = df_inputs.dropna(subset=['meal_id_suggestion']).copy()

features_and_labels = training_data[['calories', 'protein', 'min_fat', 'meal_type_id', 'meal_id_suggestion']]

# Save to CSV
features_and_labels.to_csv("./data/meal_mlp_dataset2.csv", index=False)

print(f"Dataset ready! Total samples: {len(features_and_labels)}")
            