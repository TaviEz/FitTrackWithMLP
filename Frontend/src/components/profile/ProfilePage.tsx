import { Box, Card, Paper, Stack, Typography } from "@mui/material"
import { useEffect, useState } from "react"
import { toast, ToastContainer } from "react-toastify"
import theme from "../../theme"
import UserDetails from "../../models/UserDetails"
import PersonalDetailsForm from "../onboarding/PersonalDetailsForm"
import { PrimaryButton } from "../../styledComponents/Buttons"
import { showError, showInfo } from "../shared/ShowToast"
import { saveUserDetails, getUserDetails } from "../../api/UserProfileService"
import { activityLevelsData } from "../../utils/activityLevelsData"
import { useUser } from "../../context/UserContext"

const StatCard = ({ label, value }: { label: string; value: string }) => (
    <Paper
        variant="outlined"
        sx={{
            px: 3,
            py: 1.5,
            textAlign: "center",
            borderRadius: 2,
            minWidth: 140,
            border: `1px solid ${theme.palette.primary.main}`,
            backgroundColor: theme.palette.primary.light + "18",
        }}
    >
        <Typography sx={{ ...theme.typography.caption, color: theme.palette.text.secondary }}>
            {label}
        </Typography>
        <Typography sx={{ ...theme.typography.h6, color: theme.palette.primary.main }}>
            {value}
        </Typography>
    </Paper>
)

const ProfilePage = () => {
    const { userDetails: contextDetails, setUserDetails: setUserDetailsContext } = useUser()
    const [userDetails, setUserDetails] = useState<UserDetails>(contextDetails ?? UserDetails.default())
    const [saving, setSaving] = useState(false)

    useEffect(() => {
        if (contextDetails) setUserDetails(contextDetails)
    }, [contextDetails])

    const handleOpenActivityLevel = () => {}

    const handleSave = async () => {
        if (saving) return

        if (userDetails.activityLevel.multiplier === 0) {
            showInfo("Select an activity level in order to proceed")
            return
        }

        setSaving(true)

        const genderConstant = userDetails.gender === "female" ? -161 : 5
        const bmr = 10 * userDetails.weight + 6.25 * userDetails.height - 5 * userDetails.age + genderConstant
        const tdee = Math.round(bmr * userDetails.activityLevel.multiplier)
        const withCalc = userDetails.clone()
        withCalc.bmr = bmr
        withCalc.tdee = tdee
        setUserDetails(withCalc)

        const result = await saveUserDetails(withCalc)
        if (!result.success) {
            showError(result.status === 400
                ? "Please check your personal details and try again."
                : "Could not save your details. Please try again."
            )
            setSaving(false)
            return
        }

        setUserDetailsContext(withCalc)

        const updatedDto = await getUserDetails()
        if (updatedDto.success && updatedDto.data) {
            const updated = withCalc.clone()
            updated.targetCalories = updatedDto.data.targetCalories
            setUserDetails(updated)
            setUserDetailsContext(updated)
        }

        setSaving(false)
        toast.success("Profile saved!", { position: "bottom-right", autoClose: 3000 })
    }

    const hasMetrics = userDetails.bmr !== 0 && userDetails.tdee !== 0

    return (
        <Box display="flex" flexDirection="column" alignItems="center" gap={5} p={4}>
            <Box display="flex" flexDirection="column" alignItems="center" gap={1} width="100%" maxWidth={500}>
                {hasMetrics && (
                    <Stack direction="row" spacing={2} flexWrap="wrap" justifyContent="center" width="100%">
                        <StatCard label="Target Calories" value={`${userDetails.targetCalories} kcal/day`} />
                    </Stack>
                )}

            <Card sx={{
                p: 3,
                borderRadius: 2,
                border: `1px solid ${theme.palette.divider}`,
                width: "100%",
                maxWidth: 500,
            }}>
                <PersonalDetailsForm
                    userDetails={userDetails}
                    setUserDetails={setUserDetails}
                    activityLevels={activityLevelsData}
                    handleOpenActivityLevel={handleOpenActivityLevel}
                    handleCalculateCalories={() => {}}
                    showCalculateButton={false}
                />
            </Card>
            </Box>

            <PrimaryButton onClick={handleSave} disabled={saving}>
                {saving ? "Saving..." : "Save Changes"}
            </PrimaryButton>

            <ToastContainer />
        </Box>
    )
}

export default ProfilePage
