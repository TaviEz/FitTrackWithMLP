import sqlite3
import os

def ensure_database_initialized(db_path='data/meal_planner.db'):
    # Check if database exists
    db_exists = os.path.exists(db_path)
    
    conn = sqlite3.connect(db_path)
    
    if not db_exists:
        print("Database not found. Bootstrapping from SQL scripts...")
        try:
            # Create tables and insert data
            with open('data/scripts/meal_planner.db.sql', 'r') as f:
                conn.executescript(f.read())
                
            print("Successfully initialized database.")
        except Exception as e:
            print(f"Error bootstrapping database: {e}")
            # Optional: delete the half-finished db file if it failed
            conn.close()
            os.remove(db_path)
            raise
        print(f"Database created at {db_path}")
    else:
        print("Database already exists. Skipping initialization.")
        