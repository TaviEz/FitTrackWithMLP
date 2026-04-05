import ActivityLevel from "./ActivityLevel"

class UserDetails {
    gender: string;
    age: number;
    weight: number; 
    height: number;
    activityLevel: ActivityLevel;
    calories?: number;

    public constructor(gender: string, age: number, weight: number, height: number, activityLevel: ActivityLevel)
    {
        this.gender = gender;
        this.age = age;
        this.weight = weight;
        this.height = height;
        this.activityLevel = activityLevel;
    }

    public static default() {
        return new UserDetails("male", 25, 80, 180, ActivityLevel.empty());
    }

    public clone(): UserDetails {
        return new UserDetails(
        this.gender,
        this.age,
        this.weight,
        this.height,
        this.activityLevel
        );
    }
}

export default UserDetails