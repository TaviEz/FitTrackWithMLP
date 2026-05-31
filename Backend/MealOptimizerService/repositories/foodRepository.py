import sqlite3
from payloads.foodResponse import FoodResponse


class FoodRepository:
    def __init__(self, db_path="./data/meal_planner.db"):
        self.db_path = db_path

    def search_foods_by_name(self, query: str):
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM basic_foods WHERE name LIKE ?", (f"%{query}%",))
        rows = cursor.fetchall()
        conn.close()

        return [
            FoodResponse(
                food_id=row['food_id'],
                category=row['category'],
                name=row['name'],
                calories=int(row['calories']),
                protein=row['protein'],
                fat=row['fat'],
                carbs=row['carbs']
            ) for row in rows
        ]

    def get_all_foods(self):
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM basic_foods")
        rows = cursor.fetchall()
        conn.close()

        return [
            FoodResponse(
                food_id=row['food_id'],
                category=row['category'],
                name=row['name'],
                calories=int(row['calories']),
                protein=row['protein'],
                fat=row['fat'],
                carbs=row['carbs']
            ) for row in rows
        ]