export const ActivityLevels = {
  "Sedentary": "Sedentary",
  "Lightly active": "LightlyActive",
  "Moderately active": "ModeratelyActive",
  "Very active": "VeryActive",
  "Extra active": "ExtraActive"
} as const;

export type ActivityLevelLabel = keyof typeof ActivityLevels;

export function getActivityLevelEnum(label: string): string {
  return ActivityLevels[label as ActivityLevelLabel];
}

export type NumericUserDetailsField = "age" | "weight" | "height";
