import './App.css'
import { useEffect } from 'react'
import { BrowserRouter, Route, Routes, useLocation } from 'react-router-dom'
import Login from './components/authentication/LoginPage'
import SignInForm from './components/authentication/SignInPage'
import Onboarding from './components/onboarding/OnboardingPage'
import Dashboard from './components/Dashboard'
import TopNavBar from './components/shared/TopNavBar'
import { useUser } from './context/UserContext'
import { getLoggedUserId, getUserDetails } from './api/UserProfileService'
import UserDetailsDto from './dtos/UserDetails/UserDetailsDto'

const HIDDEN_NAV_ROUTES = ['/', '/signIn', '/onboarding'];

const AppLayout = () => {
  const { pathname } = useLocation();
  const showNav = !HIDDEN_NAV_ROUTES.includes(pathname);

  return (
    <>
      {showNav && <TopNavBar />}
      <div style={{ paddingTop: showNav ? 20 : 0 }}>
        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/signIn" element={<SignInForm />} />
          <Route path="/onboarding" element={<Onboarding />} />
          <Route path="/dashboard" element={<Dashboard />} />
        </Routes>
      </div>
    </>
  );
};

const App = () => {
  const { setUserId, setUserDetails } = useUser();

  // TODO: recheck if you should store the token in localStorage
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
    <BrowserRouter>
      <AppLayout />
    </BrowserRouter>
  );
};

export default App
