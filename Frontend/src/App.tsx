import './App.css'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import Login from './components/authentication/LoginPage'
import SignInForm from './components/authentication/SignInPage'
import Onboarding from './components/onboarding/OnboardingPage'

const App = () => {
  return (
    <>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Login/>} />
          <Route path="/signIn" element={<SignInForm/>} />
          <Route path="/onboarding" element={<Onboarding/>} />
        </Routes>
      </BrowserRouter>
    </>
  )
}

export default App
