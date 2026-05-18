import ActivityLevel from "./ActivityLevel"

class UserDetails {
    gender: string;
    age: number;
    weight: number; 
    height: number;
    activityLevel: ActivityLevel;
    tdee: number;
    bmr: number;
    goal: string;
    targetCalories: number;

    public constructor(
        gender: string, age: number, weight: number, height: number, 
        activityLevel: ActivityLevel, bmr: number, tdee: number, goal: string = "", targetCalories: number)
    {
        this.gender = gender;
        this.age = age;
        this.weight = weight;
        this.height = height;
        this.activityLevel = activityLevel;
        this.bmr = bmr
        this.tdee = tdee
        this.goal = goal;
        this.targetCalories = targetCalories;
    }

    public static default() {
        return new UserDetails("male", 25, 80, 180, ActivityLevel.empty(), 1800, 2150, "", 2150);
    }

    public clone(): UserDetails {
        return new UserDetails(
            this.gender,
            this.age,
            this.weight,
            this.height,
            this.activityLevel,
            this.bmr,
            this.tdee,
            this.goal,
            this.targetCalories
        );
    }
}

export default UserDetails