import { Box, CircularProgress, Divider, Link, TextField, Typography } from "@mui/material"
import { useState } from "react"
import { PrimaryButton } from "../../styledComponents/Buttons";
import { useNavigate } from "react-router-dom";
import theme from "../../theme";
import PasswordField from "./PasswordField";
import { getLoggedUserId, loginUser } from "../../api/UserProfileService";
import { showError, showInfo } from "../shared/ShowToast";
import { ToastContainer } from 'react-toastify';
import { useUser } from "../../context/UserContext";

const Login = () => {
  const navigate = useNavigate();

  const [emailAddress, setEmailAddress] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);

  const [emailError, setEmailError] = useState('');
  const [passwordError, setPasswordError] = useState('');

  const [isLoading, setIsLoading] = useState(false);

  const {setUserId} = useUser();

  // TODO: remove comments from this handler
  const handleLogin = async () => {
      setIsLoading(true);
      const result = await loginUser(emailAddress, password);

      if(!result.success) {
        switch (result.error.status) {
          case 401:
            showError("Wrong credentials");
            break;
          default:
            showError("An error ocurred while logging in");
            break;
        }
        setIsLoading(false);
        return;
      }

      const userId = await getLoggedUserId(result.token);
      setUserId(userId);

      setIsLoading(false);
      navigate("/onboarding");
      showInfo('YEEE');
  };

  return (
    <Box display="flex" flexDirection="column" component="form" gap={3}>
      <Typography sx={{ m: 7, ...theme.typography.h2 }}>
        Login Page
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

      {isLoading ? (
          <CircularProgress sx={{ marginTop: 2, alignSelf: "center" }} />
      ) : (
          <PrimaryButton onClick={handleLogin}>Log in</PrimaryButton>
      )}

      <Divider />

      <Box display="flex" alignItems="center" justifyContent="center">
        <Typography sx={{ mx: 2, ...theme.typography.body2 }}>
          Don't have an account?
        </Typography>
        <Link href="/signIn">Sign up here</Link>
      </Box>
      <ToastContainer/>
    </Box>
  );
};


export default Login