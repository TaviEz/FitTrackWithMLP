import ActivityLevel from "../models/ActivityLevel";

export const activityLevelsData: ActivityLevel[] = [
    new ActivityLevel("Sedentary", "Little to no exercise or physical activity", 1.2),
    new ActivityLevel("Lightly active", "Light exercise or sports 1-3 days per week", 1.375),
    new ActivityLevel("Moderately active", "Moderate exercise or sports 3-5 days per week", 1.55),
    new ActivityLevel("Very active", "Hard exercise or sports 6-7 days per week", 1.725),
    new ActivityLevel("Extra active", "Very intense exercise daily or physical job", 1.9)
];

export const defaultActivityLevel = activityLevelsData[0];