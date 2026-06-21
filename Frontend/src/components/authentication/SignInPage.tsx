import { Box, TextField, Typography } from "@mui/material"
import { useEffect, useState } from "react"
import { PrimaryButton } from "../../styledComponents/Buttons";
import { useNavigate } from "react-router-dom";
import theme from "../../theme";
import PasswordField from "./PasswordField";
import { refreshSession, registerUser } from "../../api/UserProfileService";
import { showInfo } from "../shared/ShowToast";
import { ToastContainer } from "react-toastify";
import { setAccessToken } from "../../api/api";

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
    // Frontend validation
    const isValidEmail = validateEmail(emailAddress);
    const isValidConfirm = confirmPassword === password;

    const passwordValidationError = (() => {
      if (password.length < 6) return 'Password must be at least 6 characters';
      if (!/[A-Z]/.test(password)) return 'Password must have at least one uppercase letter';
      if (!/[0-9]/.test(password)) return 'Password must have at least one digit';
      if (!/[^a-zA-Z0-9]/.test(password)) return 'Password must have at least one special character';
      return '';
    })();

    setEmailError(isValidEmail ? '' : 'Enter a valid email address');
    setPasswordError(passwordValidationError);
    setConfirmError(isValidConfirm ? '' : 'Passwords do not match');

    if (!isValidEmail || passwordValidationError || !isValidConfirm) return;

    // Backend call
    const result = await registerUser(emailAddress, password);

    if (!result.success) {
      if (result.status === 409) {
        setEmailError('Email already in use');
      } else if (result.status === 400) {
        setPasswordError('Could not create account. Please check your details.');
      } else {
        setEmailError('Could not create account. Please try again.');
      }

      return;
    }

    // Success
    navigate("/");
    showInfo('Successfully registered! Please sign in with your new credentials.');
  };

    useEffect(() => {
      const checkSession = async () => {
          const result = await refreshSession();
          if (result.success) {
              setAccessToken(result.accessToken);
              navigate('/dashboard');
          }
      };
      checkSession();
    }, []);

  return (
    <Box display="flex" flexDirection="column" component="form" gap={3} sx={{ maxWidth: 480, width: "100%", mx: "auto" }}>
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
      <ToastContainer/>
    </Box>
  );
};


export default SignInForm