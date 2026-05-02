import ActivityLevel from "./ActivityLevel"

class UserDetails {
    gender: string;
    age: number;
    weight: number; 
    height: number;
    activityLevel: ActivityLevel;
    tdee: number;
    bmr: number;

    public constructor(gender: string, age: number, weight: number, height: number, activityLevel: ActivityLevel, bmr: number, tdee: number)
    {
        this.gender = gender;
        this.age = age;
        this.weight = weight;
        this.height = height;
        this.activityLevel = activityLevel;
        this.bmr = bmr
        this.tdee = tdee
    }

    public static default() {
        return new UserDetails("male", 25, 80, 180, ActivityLevel.empty(), 1800, 2150);
    }

    public clone(): UserDetails {
        return new UserDetails(
            this.gender,
            this.age,
            this.weight,
            this.height,
            this.activityLevel,
            this.bmr,
            this.tdee
        );
    }
}

export default UserDetails