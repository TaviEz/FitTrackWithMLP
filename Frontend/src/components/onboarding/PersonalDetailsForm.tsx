import { Box, FormControl, FormControlLabel, FormLabel, InputLabel, MenuItem, Radio, RadioGroup, Select, Tooltip, Typography, type SelectChangeEvent } from "@mui/material"
import InputNumber from "../../styledComponents/InputNumber"
import { PrimaryButton } from "../../styledComponents/Buttons"
import theme from "../../theme";
import ActivityLevel from "../../models/ActivityLevel";
import { type ChangeEvent } from "react";
import type UserDetails from "../../models/UserDetails";

interface PersonalDetailsFormProps {
  userDetails: UserDetails
  setUserDetails: (userDetails: UserDetails) => void;
  activityLevels: ActivityLevel[];
  handleOpenActivityLevel: () => void;
  handleCalculateCalories: () => void;
};


const PersonalDetailsForm = ({userDetails, setUserDetails, activityLevels, handleOpenActivityLevel, handleCalculateCalories}: PersonalDetailsFormProps) => {
    const handleChangeGender = (event: ChangeEvent<HTMLInputElement>) => {
        const newUserDetails = userDetails.clone();
        newUserDetails.gender = (event.target as HTMLInputElement).value;
        setUserDetails(newUserDetails);
    }

    const handleActivityChange = (event: SelectChangeEvent) => {
        const activityLabel = event.target.value as string;
        const localActivityLevel = activityLevels.find(al => al.label == activityLabel);
        if (localActivityLevel) {
            const newUserDetails = userDetails.clone();
            newUserDetails.activityLevel = localActivityLevel;
            setUserDetails(newUserDetails);
        }
    }

    return (
        <Box 
            display="flex" 
            flexDirection="column"
            justifyContent="center"
            alignItems="center"
            gap={3}
        >
            <FormControl>
                <Box display="flex" flexDirection="column" alignItems="flex-start">
                    <FormLabel
                        id="demo-row-radio-buttons-group-label"
                        sx={{
                            ...theme.typography.body1,
                            color: theme.palette.primary.main,
                            '&.Mui-focused': {
                                color: theme.palette.primary.main
                            }
                        }}
                        > Gender
                    </FormLabel>
                    <RadioGroup
                        row
                        aria-labelledby="demo-row-radio-buttons-group-label"
                        name="row-radio-buttons-group"
                        value={userDetails.gender}
                        onChange={handleChangeGender}
                    >
                        <FormControlLabel
                            value="female"
                            control={<Radio />}
                            label="Female"
                            sx={{
                                '& .MuiTypography-root': {
                                ...theme.typography.body2,
                                color: theme.palette.primary.main
                                }
                            }}
                        />
                        <FormControlLabel
                            value="male"
                            control={<Radio />}
                            label="Male"
                            sx={{
                                '& .MuiTypography-root': {
                                ...theme.typography.body2,
                                color: theme.palette.primary.main
                                }
                            }}
                        />

                        <FormControlLabel
                            value="other"
                            control={<Radio />}
                            label="Other"
                            sx={{
                                '& .MuiTypography-root': {
                                ...theme.typography.body2,
                                color: theme.palette.primary.main
                                }
                            }}
                        />
                    </RadioGroup>
                </Box>
            </FormControl>
            <InputNumber ariaLabel="age" field ="age" userDetails={userDetails} setUserDetails={setUserDetails} label="Age" maxValue={100}/>
            <InputNumber ariaLabel="weight" field ="weight" userDetails={userDetails} setUserDetails={setUserDetails} label="Weight (kg)" maxValue={200}/>
            <InputNumber ariaLabel="height" field ="height" userDetails={userDetails} setUserDetails={setUserDetails} label="Height (cm)" maxValue={250}/>
            
            <FormControl variant="standard" sx={{ m: 1, minWidth: 200 }}>
                <InputLabel id="selectActivity">Activity Level</InputLabel>
                <Select
                    value={userDetails.activityLevel.label}
                    label="Activity level"
                    onChange={handleActivityChange}
                    onOpen={handleOpenActivityLevel}
                >
                    {activityLevels.map((level) => (
                        <MenuItem key={level.label} value={level.label}>
                            <Tooltip title={level.description} placement="right" disableInteractive>
                                <Box width="100%">
                                    <Typography noWrap>{level.label}</Typography>
                                </Box>
                            </Tooltip>
                        </MenuItem>
                    ))}
                </Select>
            </FormControl>

            <PrimaryButton onClick={handleCalculateCalories}>
                Calculate
            </PrimaryButton>
        </Box>
    )
}

export default PersonalDetailsForm