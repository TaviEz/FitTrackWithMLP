from fastapi import FastAPI
from contextlib import asynccontextmanager
from pydantic import BaseModel
from MLP.prepareMLP import generate_daily_plan
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
    daily_goals: dict
    complexity: str

@app.get("/")
async def root():
    return {"status": "Optimizer API is running"}

@app.post("/optimize")
async def generate_meals(request: OptimizationRequest):
    # MLP logic
    result = generate_daily_plan(request.daily_goals)
    return {"plan": result}