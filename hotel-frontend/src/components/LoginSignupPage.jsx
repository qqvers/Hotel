import React from "react";
import { useParams } from "react-router-dom";

const LoginSignupPage = () => {
  const { action } = useParams();

  function actionHandler() {}
  return (
    <div className="mx-auto mt-24 flex w-full justify-center text-white">
      <form
        onSubmit={actionHandler}
        className="flex w-2/3 flex-col gap-1 rounded-sm border-2 border-white p-4"
      >
        {action !== "signup" && (
          <>
            <label htmlFor="name">Name</label>
            <input type="text" placeholder="Name" className="pl-1 text-black" />
          </>
        )}

        <label htmlFor="email" className="mt-4">
          Email
        </label>
        <input type="text" placeholder="Email" className="pl-1 text-black" />

        <label htmlFor="email" className="mt-4">
          Password
        </label>
        <input
          type="password"
          placeholder="Password"
          className="pl-1 text-black"
        />

        {action === "signup" && (
          <div className="flex h-8 items-center">
            <input
              type="checkbox"
              placeholder="Password"
              className="pl-1 text-black"
            />
            <label htmlFor="checkbox" className="ml-2">
              Are you an owner?
            </label>
          </div>
        )}
        <button
          type="submit"
          className="mx-auto mt-4 w-fit rounded-md border-2 border-white px-4 py-1"
        >
          {action === "signup" ? "Sign up" : "Login"}
        </button>
      </form>
    </div>
  );
};

export default LoginSignupPage;
