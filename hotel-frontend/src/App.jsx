import Navbar from "./components/Navbar";
import HomePage from "./components/HomePage";
import LoginSignupPage from "./components/LoginSignupPage";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { useState } from "react";
import ProfilePage from "./components/ProfilePage";
import RoomPage from "./components/RoomPage";

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const handleLogin = () => {
    setIsLoggedIn(true);
  };

  return (
    <Router>
      <div className="h-[1920px] w-full bg-black">
        <Navbar isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/profile" element={<ProfilePage />} />
          <Route path="/room/" element={<RoomPage />} />
          <Route path="/room/:roomId" element={<RoomPage />} />
          <Route
            path="/form/:action"
            element={<LoginSignupPage handleLogin={handleLogin} />}
          />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
