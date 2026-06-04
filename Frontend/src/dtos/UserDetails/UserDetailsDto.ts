import UserDetails from "../../models/UserDetails";
import { activityLevelsData, defaultActivityLevel } from "../../utils/activityLevelsData";
import { ActivityLevels, type ActivityLevelLabel } from "../../utils/types";

class UserDetailsDto {
    gender: string;
    age: number;
    weight: number; 
    height: number;
    activityLevel: string;
    bmr: number;
    tdee: number;
    goalType: string;
    targetCalories: number;

    public constructor(
        gender: string, age: number, weight: number, height: number, 
        activityLevel: string, bmr: number, tdee: number, goalType: string, targetCalories: number)
    {
        this.gender = gender;
        this.age = age;
        this.weight = weight;
        this.height = height;
        this.activityLevel = activityLevel;
        this.bmr = bmr;
        this.tdee = tdee;
        this.goalType = goalType;
        this.targetCalories = targetCalories;
    }

    static fromDomain(details: any, activityLevelEnum: string): UserDetailsDto {
        return new UserDetailsDto(
            details.gender,
            details.age,
            Math.round(details.weight),
            Math.round(details.height),
            activityLevelEnum,
            Math.round(details.bmr),
            Math.round(details.tdee),
            details.goal,
            details.targetCalories
        );
    }

    static toDomain(dto: UserDetailsDto): UserDetails {
        const label = Object.keys(ActivityLevels).find(
            (key) => ActivityLevels[key as ActivityLevelLabel].toLowerCase() === dto.activityLevel?.toLowerCase()
        );
        const activityLevel = activityLevelsData.find(a => a.label === label) ?? defaultActivityLevel;
        return new UserDetails(
            dto.gender?.toLowerCase() ?? "",
            dto.age, 
            dto.weight, 
            dto.height, 
            activityLevel, 
            dto.bmr, 
            dto.tdee, 
            dto.goalType,
            dto.targetCalories
        );
    }
}

export default UserDetailsDto