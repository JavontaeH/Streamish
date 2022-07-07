import React, { useEffect, useState } from "react";
import Video from "./Video";
import { getAllVideos, searchVideos } from "../modules/videoManager";

const VideoList = () => {
  const [videos, setVideos] = useState([]);

  const getVideos = () => {
    getAllVideos().then((videos) => setVideos(videos));
  };

  useEffect(() => {
    getVideos();
  }, []);

  let handleFieldChange = (evt) => {
    searchVideos(evt.target.value).then((videos) => setVideos(videos));
  };

  return (
    <div className="container">
      <div className="search-container">
        <input
          type="text"
          className="outlined-basic"
          onChange={handleFieldChange}
          id="outlined-basic"
        ></input>
      </div>
      <div className="row justify-content-center">
        {videos.map((video) => (
          <Video video={video} key={video.id} />
        ))}
      </div>
    </div>
  );
};

export default VideoList;
