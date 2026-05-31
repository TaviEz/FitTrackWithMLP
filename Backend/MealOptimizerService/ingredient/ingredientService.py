from repositories.foodRepository import FoodRepository

def get_ingredient_options(query: str) -> list:
    foodRepository = FoodRepository()
    return foodRepository.search_foods_by_name(query)