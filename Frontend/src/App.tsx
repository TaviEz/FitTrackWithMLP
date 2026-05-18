import './App.css'
import { useEffect } from 'react'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import Login from './components/authentication/LoginPage'
import SignInForm from './components/authentication/SignInPage'
import Onboarding from './components/onboarding/OnboardingPage'
import Dashboard from './components/Dashboard'
import { useUser } from './context/UserContext'
import { getLoggedUserId, getUserDetails } from './api/UserProfileService'
import UserDetailsDto from './dtos/UserDetails/UserDetailsDto'

const App = () => {
  const { setUserId, setUserDetails } = useUser();

  useEffect(() => {
    const rehydrate = async () => {
      const token = localStorage.getItem('token');
      if (!token) return;

      const userId = await getLoggedUserId(token);
      if (userId) setUserId(userId);

      const dto = await getUserDetails();
      if (dto) setUserDetails(UserDetailsDto.toDomain(dto));
    };
    rehydrate();
  }, []);

  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Login/>} />
          <Route path="/signIn" element={<SignInForm/>} />
          <Route path="/onboarding" element={<Onboarding/>} />
          <Route path="/dashboard" element={<Dashboard/>} />
        </Routes>
      </BrowserRouter>
    </>
  )
}

export default App
