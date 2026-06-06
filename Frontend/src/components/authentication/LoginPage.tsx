import { Box, CircularProgress, Divider, Link, TextField, Typography } from "@mui/material"
import { useState } from "react"
import { PrimaryButton } from "../../styledComponents/Buttons";
import { useNavigate } from "react-router-dom";
import theme from "../../theme";
import PasswordField from "./PasswordField";
import { getLoggedUserId, getUserDetails, loginUser } from "../../api/UserProfileService";
import { setAccessToken } from "../../api/api";
import { showError } from "../shared/ShowToast";
import { ToastContainer } from 'react-toastify';
import { useUser } from "../../context/UserContext";
import UserDetailsDto from "../../dtos/UserDetails/UserDetailsDto";

const Login = () => {
  const navigate = useNavigate();

  const [emailAddress, setEmailAddress] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);

  const [emailError, setEmailError] = useState('');
  const [passwordError, setPasswordError] = useState('');

  const [isLoading, setIsLoading] = useState(false);

  const {setUserId, setUserDetails} = useUser();

  const handleLogin = async () => {
      setIsLoading(true);
      const result = await loginUser(emailAddress, password);

      if(!result.success) {
        switch (result.status) {
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

      setAccessToken(result.accessToken);

      const userIdResult = await getLoggedUserId();
      setUserId(userIdResult.data);

      const userDetailsResult = await getUserDetails();
      setIsLoading(false);

      // in case we already have the user details of the user
      // directly navigate to the dashboard page
      if (userDetailsResult.success && userDetailsResult.data)
      {
        const userDetails = UserDetailsDto.toDomain(userDetailsResult.data);
        setUserDetails(userDetails);
        navigate("/dashboard");
        return;
      }

      navigate("/onboarding");
  };

  return (
    <Box display="flex" flexDirection="column" component="form" gap={3} sx={{ maxWidth: 480, width: "100%", mx: "auto" }}>
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