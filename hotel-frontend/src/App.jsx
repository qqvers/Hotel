import Navbar from "./components/Navbar";
import HomePage from "./components/HomePage";
import LoginSignupPage from "./components/LoginSignupPage";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";

function App() {
  return (
    <Router>
      <div className="h-screen w-screen bg-black">
        <Navbar />
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/form/:action" element={<LoginSignupPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
