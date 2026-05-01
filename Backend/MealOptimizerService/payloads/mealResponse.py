class MealResponse:
    def __init__(self, meal_id, meal_type_id, title, complexity, ingredients):
        self.id = meal_id
        self.meal_type_id = meal_type_id
        self.title = title
        self.complexity = complexity
        self.ingredients = ingredients