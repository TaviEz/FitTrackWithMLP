from MLP.prepareMLP import generate_daily_plan

test_scenarios_v2 = {
    "ultra_lean_shred": {
        "cal": 1800,
        "p": 160,
        "f_min": 40
    },
    "high_fat_keto_style": {
        "cal": 2800,
        "p": 120,
        "f_min": 150
    },
    "carb_loading_endurance": {
        "cal": 3500,
        "p": 100,
        "f_min": 60
    },
    "small_female_maintenance": {
        "cal": 1600,
        "p": 90,
        "f_min": 55
    },
    "heavy_duty_power_bulk": {
        "cal": 4500,
        "p": 200,
        "f_min": 120
    }
}

for name, goals in test_scenarios_v2.items():
    result = generate_daily_plan(goals)

    print("\n" + "="*50)
    print(f"  GENERATED MEAL PLAN | GOAL: {goals['cal']} kcal")
    print("="*50)

    for meal in result:
        # Header for the meal category
        print(f"\n▶ {meal['category']}: {meal['title']}")
        print(f"  (Meal ID: {meal['meal_id']} | Optimization Error: {meal['error']:.4f})")
        print("  " + "-"*30)
        
        # List the ingredients
        for ing in meal['ingredients']:
            # Format: - 150g Ingredient Name
            print(f"  - {ing['amount_g']:>4}g {ing['name']}")

    print("\n" + "="*50)