class DailyPlanMealResponse:
    def __init__(self, category, title, meal_id, ingredients=None, error=0.0):
        self.category = category
        self.title = title
        self.meal_id = meal_id
        self.ingredients = ingredients if ingredients is not None else []
        self.error = error
