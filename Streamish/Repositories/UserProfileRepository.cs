using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Models;
using Streamish.Utils;

namespace Streamish.Repositories
{

    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration configuration) : base(configuration) { }

        public List<UserProfile> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    select u.id, u.name, u.imageurl, u.datecreated, u.email
                    from userprofile u
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        var userprofiles = new List<UserProfile>();
                        while (reader.Read())
                        {
                            userprofiles.Add(new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                Email = DbUtils.GetString(reader, "Email"),
                            });
                        }
                        return userprofiles;
                    }
                }
            }
        }

        public UserProfile GetById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    select u.id, u.name, u.imageurl, u.datecreated, u.email
                    from userprofile u
                    where Id = @Id
                    ";
                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        UserProfile userprofile = null;
                        if (reader.Read())
                        {
                            userprofile = (new UserProfile()
                            {
                                Id = DbUtils.GetInt(reader, "Id"),
                                Name = DbUtils.GetString(reader, "Name"),
                                ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                Email = DbUtils.GetString(reader, "Email"),
                            });
                        }
                        return userprofile;
                    }
                }
            }
        }

        public UserProfile GetByIdWithVideos(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    select u.id, u.name, u.imageurl, u.datecreated, u.email, v.id as 'VideoId', v.description, v.datecreated as 'VideoDateCreated', v.url, v.title, c.id as 'CommentId', c.message, c.UserProfileId as 'CommentUserProfileId'
                    from userprofile u
                    join video v on v.UserProfileId = u.id and u.id = @id
                    left join comment c on c.VideoId = v.id;
                    ";
                    DbUtils.AddParameter(cmd, "@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        UserProfile userprofile = null;
                        Video existingVideo = null;
                        
                        while (reader.Read())
                        {
                            if (userprofile == null)
                            {
                                userprofile = (new UserProfile()
                                {
                                    Id = DbUtils.GetInt(reader, "Id"),
                                    Name = DbUtils.GetString(reader, "Name"),
                                    ImageUrl = DbUtils.GetString(reader, "ImageUrl"),
                                    DateCreated = DbUtils.GetDateTime(reader, "DateCreated"),
                                    Email = DbUtils.GetString(reader, "Email"),
                                    Videos = new List<Video>(),
                                });
                            }
                            if (DbUtils.IsNotDbNull(reader, "VideoId"))
                            {
                            var videoId = DbUtils.GetInt(reader, "VideoId");
                            existingVideo = userprofile.Videos.FirstOrDefault(p => p.Id == videoId);
                                if (existingVideo == null)
                                {
                                    existingVideo = new Video()
                                    {
                                        Id = DbUtils.GetInt(reader, "VideoId"),
                                        Description = DbUtils.GetString(reader, "Description"),
                                        UserProfileId = id,
                                        DateCreated = DbUtils.GetDateTime(reader, "VideoDateCreated"),
                                        Url = DbUtils.GetString(reader, "Url"),
                                        Title = DbUtils.GetString(reader, "Title"),
                                        Comments = new List<Comment>(),

                                    };
                                }
                                userprofile.Videos.Add(existingVideo);
                            }
                            if (DbUtils.IsNotDbNull(reader, "CommentId"))
                            {
                                existingVideo.Comments.Add(new Comment
                                {
                                    Id = DbUtils.GetInt(reader, "CommentId"),
                                    Message = DbUtils.GetString(reader, "message"),
                                    UserProfileId = DbUtils.GetInt(reader, "CommentUserProfileId"),
                                    VideoId = existingVideo.Id,





                                });
                            }


                        }
                        return userprofile;
                    }
                }
            }
        }
        public void Add(UserProfile userProfile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO UserProfile (Name, Email, DateCreated, ImageUrl)
                        OUTPUT INSERTED.ID
                        VALUES (@Name, @Email, @DateCreated, @ImageUrl)";

                    DbUtils.AddParameter(cmd, "@Name", userProfile.Name);
                    DbUtils.AddParameter(cmd, "@Email", userProfile.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", userProfile.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", userProfile.ImageUrl);


                    userProfile.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(UserProfile userProfile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile
                        SET Name = @Name,
                        Email = @Email,
                        ImageUrl = @ImageUrl,
                        DateCreated = @DateCreated
                        WHERE Id = @Id";

                    DbUtils.AddParameter(cmd, "@Name", userProfile.Name);
                    DbUtils.AddParameter(cmd, "@Email", userProfile.Email);
                    DbUtils.AddParameter(cmd, "@DateCreated", userProfile.DateCreated);
                    DbUtils.AddParameter(cmd, "@ImageUrl", userProfile.ImageUrl);
                    DbUtils.AddParameter(cmd, "@Id", userProfile.Id);


                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM UserProfile WHERE Id = @Id";
                    DbUtils.AddParameter(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}