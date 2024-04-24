import React from "react";
import { Link } from "react-router-dom";

const Navbar = () => {
  return (
    <div className="flex h-16 w-full items-center justify-end border-b-2 border-gray-200 text-white">
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
    </div>
  );
};

export default Navbar;
