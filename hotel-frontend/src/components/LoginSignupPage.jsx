import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

const LoginSignupPage = ({ handleLogin }) => {
  const { action } = useParams();
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isOwner, setIsOwner] = useState(false);
  const [fetchSucces, setFetchSuccess] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    setFetchSuccess("");
    setName("");
    setEmail("");
    setPassword("");
    setIsOwner(false);
  }, [action]);

  async function actionHandler(event) {
    event.preventDefault();
    try {
      const requestBody = {
        Email: email,
        Password: password,
      };
      if (action === "signup") {
        requestBody.Name = name;
      }

      var endpointURL = isOwner ? "owner" : "customer";
      const response = await fetch(
        `https://localhost:7108/api/${endpointURL}s/${action}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(requestBody),
        },
      );
      setFetchSuccess("data fetched");
      if (action === "login") {
        const data = await response.json();
        localStorage.setItem("token", data.token);
        handleLogin();
        navigate("/");
      }
    } catch (err) {
      setFetchSuccess("data not fetched");
      throw err;
    }
  }
  return (
    <div className="mx-auto mt-24 flex w-full max-w-[600px] flex-col items-center text-white">
      <h1 className="text-3xl font-bold">
        {action === "signup" ? "Sign up" : "Login"}
      </h1>
      <form
        onSubmit={actionHandler}
        className="mt-8 flex w-2/3 flex-col gap-1 rounded-sm border-2 border-white p-4"
      >
        {action === "signup" && (
          <>
            <label htmlFor="name">Name</label>
            <input
              required
              type="text"
              placeholder="Name"
              className="pl-1 text-black"
              value={name}
              onChange={(e) => setName(e.target.value)}
            />
          </>
        )}

        <label htmlFor="email">Email</label>
        <input
          required
          type="text"
          placeholder="Email"
          className="pl-1 text-black"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />

        <label htmlFor="password">Password</label>
        <input
          required
          type="password"
          placeholder="Password"
          className="pl-1 text-black"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />

        <div className="flex h-8 items-center">
          <input
            type="checkbox"
            checked={isOwner}
            onChange={(e) => setIsOwner(e.target.checked)}
          />
          <label htmlFor="checkbox" className="ml-2">
            Are you an owner?
          </label>
        </div>

        <button
          type="submit"
          className="mx-auto mt-4 w-fit rounded-md border-2 border-white px-4 py-1"
        >
          {action === "signup" ? "Sign up" : "Login"}
        </button>
        <p>
          {fetchSucces === "data fetched" && "Account created"}
          {fetchSucces === "data not fetched" &&
            "Some error occured, data not fetched"}
        </p>
      </form>
    </div>
  );
};

export default LoginSignupPage;
