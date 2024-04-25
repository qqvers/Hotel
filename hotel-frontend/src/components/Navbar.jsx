import React, { useEffect } from "react";
import { Link } from "react-router-dom";
import { jwtDecode } from "jwt-decode";

const Navbar = ({ isLoggedIn, setIsLoggedIn }) => {
  const token = localStorage.getItem("token");
  const decodedToken = token && jwtDecode(token);
  const isOwner = token && decodedToken.UserType === "Owner";

  useEffect(() => {
    const token = localStorage.getItem("token");
    setIsLoggedIn(!!token);
  }, [isLoggedIn]);

  const handleLogout = () => {
    localStorage.removeItem("token");
    setIsLoggedIn(false);
  };

  return (
    <div className="flex h-16 w-full items-center justify-end border-b-2 border-gray-200 text-white">
      {!isLoggedIn ? (
        <>
          <Link
            to="/form/login"
            className="mr-8 flex h-8 cursor-pointer items-center justify-center rounded-md border-[1px] border-white p-2"
          >
            Login
          </Link>
          <Link
            to="/form/signup"
            className="mr-8 flex h-8 cursor-pointer items-center justify-center rounded-md border-[1px] border-white p-2"
          >
            Sign up
          </Link>
        </>
      ) : (
        <>
          <Link
            to="/"
            className="mr-8 flex h-8 cursor-pointer items-center justify-center rounded-md border-[1px] border-white p-2"
          >
            Home
          </Link>
          <Link
            to="/profile"
            className="mr-8 flex h-8 cursor-pointer items-center justify-center rounded-md border-[1px] border-white p-2"
          >
            Edit Profile
          </Link>

          {isOwner && (
            <Link
              to="/room"
              className="mr-8 flex h-8 cursor-pointer items-center justify-center rounded-md border-[1px] border-white p-2"
            >
              Add Room
            </Link>
          )}
          <Link
            to="/"
            className="mr-8 flex h-8 cursor-pointer items-center justify-center rounded-md border-[1px] border-white p-2"
            onClick={handleLogout}
          >
            Logout
          </Link>
        </>
      )}
    </div>
  );
};

export default Navbar;
