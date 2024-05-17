import React from "react";
import { jwtDecode } from "jwt-decode";
import { useState } from "react";

const ProfilePage = () => {
  const token = localStorage.getItem("token");
  const decodedToken = jwtDecode(token);
  const isOwner = token && decodedToken.UserType === "Owner";
  const [name, setName] = useState(decodedToken.unique_name);
  const [email, setEmail] = useState(decodedToken.nameid);
  const [password, setPassword] = useState("");
  const [fetchSucces, setFetchSuccess] = useState("");

  async function editHandler(event) {
    event.preventDefault();
    const URL = isOwner
      ? `https://localhost:7108/api/owners/${decodedToken.Id}`
      : `https://localhost:7108/api/customers/${decodedToken.Id}`;
    try {
      const response = await fetch(URL, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
          name: name,
          email: email,
          password: password,
        }),
      });
      if (response.ok) {
        setFetchSuccess("data fetched");
      }
    } catch (err) {
      setFetchSuccess("data not fetched");
      throw (err, "Failed to update profile");
    }
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

        <button
          type="submit"
          className="mx-auto mt-4 w-fit rounded-md border-2 border-white px-4 py-1"
        >
          Save changes
        </button>

        <p>
          {fetchSucces === "data fetched" && "Profile updated successfully"}
          {fetchSucces === "data not fetched" &&
            "Some error occured, data not fetched"}
        </p>
      </form>
    </div>
  );
};

export default ProfilePage;
