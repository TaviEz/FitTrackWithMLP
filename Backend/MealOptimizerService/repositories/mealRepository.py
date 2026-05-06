import sqlite3
import os

from payloads.ingredientRespose import IngredientResponse
from payloads.mealResponse import MealResponse

class MealRepository:
    def __init__(self, db_path="./data/meal_planner.db"):
        self.db_path = db_path

    def get_all_meals_details(self):
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row
        cursor = conn.cursor()

        # Query all meals and their linked ingredients
        query = """
            SELECT m.meal_id, m.meal_type_id, m.title, m.complexity, f.name
            FROM meals m
            JOIN meal_ingredients mi ON m.meal_id = mi.meal_id
            JOIN basic_foods f ON mi.food_id = f.food_id
            ORDER BY m.meal_id
        """
        cursor.execute(query)
        rows = cursor.fetchall()
        conn.close()

        # Dictionary to temporarily group ingredients by meal_id
        # Key: meal_id, Value: { "title": str, "complexity": str, "ingredients": [] }
        meals_map = {}

        for row in rows:
            m_id = row['meal_id']
            
            if m_id not in meals_map:
                meals_map[m_id] = {
                    "id": m_id,
                    "mealTypeId": row["meal_type_id"],
                    "title": row['title'],
                    "complexity": row['complexity'],
                    "ingredients": []
                }
            
            # Add the ingredient name to the list for this specific meal
            meals_map[m_id]["ingredients"].append(row['name'])

        # Convert the map back into a list of your DTOs (MealResponse)
        return [
            MealResponse(
                meal_id=data["id"],
                title=data["title"],
                meal_type_id=data["mealTypeId"],
                complexity=data["complexity"],
                ingredients=data["ingredients"]
            ) for data in meals_map.values()
        ]

    def get_meal_details_by_id(self, meal_id):
        meal_id = int(meal_id)
        conn = sqlite3.connect(self.db_path)
        conn.execute("PRAGMA wal_checkpoint(FULL);")
        conn.row_factory = sqlite3.Row
        cursor = conn.cursor()

        query = """
            SELECT m.meal_id, m.meal_type_id, m.title, m.complexity, f.food_id, f.name
            FROM meals m
            JOIN meal_ingredients mi ON m.meal_id = mi.meal_id
            JOIN basic_foods f ON mi.food_id = f.food_id
            WHERE m.meal_id = ?
        """
        
        cursor.execute(query, (meal_id,))
        rows = cursor.fetchall()
        conn.close()

        if not rows:
            return None

        # Extract general meal info from the first row
        first_row = rows[0]
        
        ingredients = [
            IngredientResponse(food_id=row['food_id'], name=row['name'])
            for row in rows
            if row['name'] is not None
        ]

        return MealResponse(
            meal_id=first_row['meal_id'],
            meal_type_id=first_row['meal_type_id'],
            title=first_row['title'],
            complexity=first_row['complexity'],
            ingredients=ingredients
        )