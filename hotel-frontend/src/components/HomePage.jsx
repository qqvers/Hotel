import React, { useEffect, useState } from "react";
import { jwtDecode } from "jwt-decode";
import { Link } from "react-router-dom";

const HomePage = () => {
  const [rooms, setRooms] = useState([]);
  const token = localStorage.getItem("token");
  const decodedToken = token && jwtDecode(token);
  const isOwner = token && decodedToken.UserType === "Owner";

  async function GetRooms() {
    try {
      const response = await fetch("https://localhost:7108/api/Rooms/allrooms");
      const data = await response.json();
      setRooms(data);
    } catch (err) {
      throw (err, "Failed to fetch rooms");
    }
  }

  useEffect(() => {
    if (token) {
      GetRooms();
    }
  }, []);

  async function DeleteRoom(id) {
    try {
      const response = await fetch(
        `https://localhost:7108/api/Rooms/deleteroom/${id}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        },
      );
      if (response.ok) {
        GetRooms();
      }
      setRooms(data);
    } catch (err) {
      throw (err, "Failed to delete room");
    }
  }

  async function RentRoom(id) {
    try {
      const response = await fetch(
        `https://localhost:7108/api/Rooms/rentroom/${id}/${decodedToken.Id}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        },
      );

      if (!response.ok) {
        throw (err, "Bad response from server");
      }

      GetRooms();
    } catch (err) {
      throw (err, "Failed to rent room");
    }
  }

  return (
    <div className="grid h-fit w-full grid-cols-1 justify-items-center gap-12 pt-8 text-white md:grid-cols-2 lg:grid-cols-3 2xl:grid-cols-4">
      {token &&
        rooms.map((room) => (
          <div
            key={room.id}
            className={`flex h-48 w-72 flex-col items-center justify-between rounded-full border-2 ${room.available ? "border-green-500" : "border-red-500"} pt-4 text-center text-lg font-bold`}
          >
            {room.name}
            <div className="mb-16">
              {isOwner ? (
                <>
                  <Link
                    to={`/room/${room.id}`}
                    className="mx-1 rounded-full border-2 border-white p-2"
                  >
                    Edit
                  </Link>
                  <button
                    className="mx-1 rounded-full border-2 border-white p-2"
                    onClick={() => DeleteRoom(room.id)}
                  >
                    Delete
                  </button>
                </>
              ) : (
                <button
                  className="mx-1 rounded-full border-2 border-white p-2"
                  onClick={() => RentRoom(room.id)}
                >
                  Rent
                </button>
              )}
            </div>
          </div>
        ))}
    </div>
  );
};

export default HomePage;
