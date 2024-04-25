import React from "react";
import { jwtDecode } from "jwt-decode";
import { useState } from "react";
import { useParams } from "react-router-dom";

const RoomPage = () => {
  const token = localStorage.getItem("token");
  const decodedToken = jwtDecode(token);
  const isOwner = token && decodedToken.UserType === "Owner";
  const [name, setName] = useState("");
  const [fetchSucces, setFetchSuccess] = useState("");
  const { roomId } = useParams();

  async function AddRoom(event) {
    event.preventDefault();
    try {
      const response = await fetch(
        "https://localhost:7108/api/Rooms/createroom",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            name: name,
            available: true,
            ownerId: decodedToken.Id,
          }),
        },
      );
      if (response.ok) {
        setFetchSuccess("data fetched");
      }
    } catch (err) {
      throw (err, "Failed to fetch rooms");
    }
  }

  async function editHandler(event) {
    event.preventDefault();
    try {
      const response = await fetch(
        `https://localhost:7108/api/Rooms/update/${roomId}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            name: name,
            avaiable: true,
            ownerId: decodedToken.Id,
          }),
        },
      );
      if (response.ok) {
        setFetchSuccess("data fetched");
      }
    } catch (err) {
      throw (err, "Failed to fetch rooms");
    }
  }

  return (
    <div className="mx-auto mt-24 flex w-full max-w-[600px] flex-col items-center text-white">
      <form
        onSubmit={roomId ? editHandler : AddRoom}
        className="mt-8 flex w-2/3 flex-col gap-1 rounded-sm border-2 border-white p-4"
      >
        {roomId && (
          <>
            <label htmlFor="id">ID</label>
            <input
              required
              type="text"
              className="pl-1 text-black"
              value={roomId}
              readOnly
            />
          </>
        )}

        <label htmlFor="name">Name</label>
        <input
          required
          type="text"
          placeholder="Name"
          className="pl-1 text-black"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />

        <button
          type="submit"
          className="mx-auto mt-4 w-fit rounded-md border-2 border-white px-4 py-1"
        >
          {isOwner && roomId ? "Edit room" : "Add room"}
        </button>

        <p>
          {fetchSucces === "data fetched" && "Success!"}
          {fetchSucces === "data not fetched" &&
            "Some error occured, data not fetched"}
        </p>
      </form>
    </div>
  );
};

export default RoomPage;
