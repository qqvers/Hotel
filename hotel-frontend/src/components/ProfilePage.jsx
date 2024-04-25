import React from "react";
import { jwtDecode } from "jwt-decode";
import { useState } from "react";

const ProfilePage = () => {
  const token = localStorage.getItem("token");
  const decodedToken = jwtDecode(token);
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [repeatPassword, setRepeatPassword] = useState("");
  const [fetchSucces, setFetchSuccess] = useState("");

  async function editHandler(event) {
    event.preventDefault();
  }

  return (
    <div className="mx-auto mt-24 flex w-full max-w-[600px] flex-col items-center text-white">
      <form
        onSubmit={editHandler}
        className="mt-8 flex w-2/3 flex-col gap-1 rounded-sm border-2 border-white p-4"
      >
        <label htmlFor="name">Name</label>
        <input
          required
          type="text"
          placeholder="Name"
          className="pl-1 text-black"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />

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

        <label htmlFor="password">Repeat Password</label>
        <input
          required
          type="password"
          placeholder="Repeat Password"
          className="pl-1 text-black"
          value={repeatPassword}
          onChange={(e) => setRepeatPassword(e.target.value)}
        />

        <button
          type="submit"
          className="mx-auto mt-4 w-fit rounded-md border-2 border-white px-4 py-1"
        >
          Save changes
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

export default ProfilePage;
