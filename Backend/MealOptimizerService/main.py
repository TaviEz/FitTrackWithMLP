from fastapi import FastAPI
from contextlib import asynccontextmanager
from fastapi.responses import JSONResponse
from pydantic import BaseModel
from MLP.prepareMLP import generate_daily_plan
from ingredient.ingredientService import get_ingredient_options
from payloads.mealTargets import MealTargets
from helpers.prepareDbConnection import ensure_database_initialized

@asynccontextmanager
async def lifespan(app: FastAPI):
    # This runs when the API starts
    print("Checking database status...")
    ensure_database_initialized('data/meal_planner.db')
    yield
    # This runs when the API shuts down
    print("Shutting down Optimizer API.")

app = FastAPI(lifespan=lifespan)

class OptimizationRequest(BaseModel):
    calories: float
    protein: float
    min_fat: float
    meals_complexity: str = "Standard"
    excluded_meal_ids: list[int] = []

@app.get("/")
async def root():
    return {"status": "Optimizer API is running"}

@app.post("/optimize")
async def generate_meals(request: OptimizationRequest):
    goals = MealTargets(
        calories=request.calories,
        protein=request.protein,
        min_fat=request.min_fat
    )
    result = generate_daily_plan(goals, request.meals_complexity, request.excluded_meal_ids)
    
    # handle meals that fail to be optimized due to infeasible_constraints
    if isinstance(result, dict) and result.get("error") == "INFEASIBLE_CONSTRAINTS":
        return JSONResponse(
            status_code=422,
            content=result
        )
        
    return result

@app.get("/ingredients")
async def get_ingredients(query: str = ""):
    return get_ingredient_options(query)