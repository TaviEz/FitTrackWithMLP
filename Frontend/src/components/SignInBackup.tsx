import { Box, TextField, Typography } from "@mui/material"
import { useState } from "react"
import { PrimaryButton } from "../styledComponents/Buttons";
import { useNavigate } from "react-router-dom";
import theme from "../theme";
import PasswordField from "./authentication/PasswordField";
import { registerUser } from "../api/UserProfileService";
import { showInfo } from "./shared/ShowToast";

const SignInForm = () => {
  const navigate = useNavigate();

  const [emailAddress, setEmailAddress] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);

  const [emailError, setEmailError] = useState('');
  const [passwordError, setPasswordError] = useState('');
  const [confirmError, setConfirmError] = useState('');

  const validateEmail = (email: string) =>
    /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);

  const handleSignIn = async () => {
    const isValidEmail = validateEmail(emailAddress);
    const isValidPassword = password.length >= 6;
    const isValidConfirm = confirmPassword === password;

    setEmailError(isValidEmail ? '' : 'Enter a valid email address');
    setPasswordError(isValidPassword ? '' : 'Password must be at least 6 characters');
    setConfirmError(isValidConfirm ? '' : 'Passwords do not match');

    if (!isValidEmail || !isValidPassword || !isValidConfirm) return;

    const result = await registerUser(emailAddress, password);
    if (result.success) {
      navigate("/onboarding");
      showInfo('YEEE');
    }

    navigate("/onboarding");
  };

  return (
    <Box display="flex" flexDirection="column" component="form" gap={3}>
      <Typography sx={{ m: 7, ...theme.typography.h2 }}>
        Sign In Page
      </Typography>

      <TextField
        label="Email Address"
        variant="standard"
        required
        fullWidth
        value={emailAddress}
        onChange={(e) => {
          setEmailAddress(e.target.value);
          setEmailError('');
        }}
        error={!!emailError}
        helperText={emailError}
      />

      <PasswordField
        label="Password"
        value={password}
        onChange={(e) => {
          setPassword(e.target.value);
          setPasswordError('');
        }}
        error={!!passwordError}
        helperText={passwordError}
        show={showPassword}
        toggleShow={() => setShowPassword((prev) => !prev)}
      />

      <PasswordField
        label="Confirm Password"
        value={confirmPassword}
        onChange={(e) => {
          setConfirmPassword(e.target.value);
          setConfirmError('');
        }}
        error={!!confirmError}
        helperText={confirmError}
        show={showConfirm}
        toggleShow={() => setShowConfirm((prev) => !prev)}
      />

      <PrimaryButton onClick={handleSignIn}>Sign In</PrimaryButton>
    </Box>
  );
};


export default SignInForm