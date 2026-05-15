class DailyPlanMealResponse:
    def __init__(self, category, title, mealId, ingredients=None, error=0.0, calories=0.0):
        self.category = category
        self.title = title
        self.mealId = mealId
        self.ingredients = ingredients if ingredients is not None else []
        self.error = error
        self.calories = calories
