using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Project.Models;
using System.Data.SqlClient;

namespace Social_Media_Project.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UtilityAPIController : ControllerBase
    {
        private readonly string _connectionString;


        public UtilityAPIController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CustomConnection");

        }


        #region "Like Post"
        public IActionResult LikePost([FromBody] MediaPost obj)
        {
            string Message = "";
            bool IsLiked = false;
            bool response = false;
            try
            {
                if (obj.PostId != null && obj.Username != null && obj.Id != null && obj.HdnUsername != null)
                {

                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {

                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_UserLikeGet", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Mode", 1);
                            cmd.Parameters.AddWithValue("@PostId", obj.PostId);
                            cmd.Parameters.AddWithValue("@PostUserName", obj.Username);
                            cmd.Parameters.AddWithValue("@LikeId", obj.Id);
                            cmd.Parameters.AddWithValue("@LikeUserName", obj.HdnUsername);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    IsLiked = reader.GetInt32(0) == 1;
                                }
                            }
                        };

                        if (IsLiked)
                        {
                            using (SqlCommand cmd = new SqlCommand("usp_UserLikeGet", con))
                            {
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@Mode", 3);
                                cmd.Parameters.AddWithValue("@LikeId", obj.Id);
                                cmd.Parameters.AddWithValue("@PostId", obj.PostId);

                                cmd.ExecuteNonQuery();
                                Message = "Unliked post successfully";
                                IsLiked = false;
                            }
                        }
                        else
                        {
                            using (SqlCommand cmd = new SqlCommand("usp_UserLikeGet", con))
                            {
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Mode", 2);
                                cmd.Parameters.AddWithValue("@PostId", obj.PostId);
                                cmd.Parameters.AddWithValue("@PostUserName", obj.Username);
                                cmd.Parameters.AddWithValue("@LikeId", obj.Id);
                                cmd.Parameters.AddWithValue("@LikeUserName", obj.HdnUsername);

                                cmd.ExecuteNonQuery();
                                Message = "Post liked successfully";
                                IsLiked = true;
                                response = true;
                            }
                        }


                    }
                }
                else
                {
                    Message = "Please pass the required parameters";
                    response = false;

                }
            }
            catch (Exception ex)
            {
                response = false;
                Message = ex.Message;
            }
            return Ok(new { msg = Message, res = response, isLike = IsLiked });

        }

        #endregion

        #region "Check Like Status"

        [HttpPost]
        public IActionResult CheckLikeStatus([FromBody] MediaPost obj)
        {
            string Message = "";
            bool response = false;
            bool IsLiked = false;
            try
            {
                if (obj.PostId != null)
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("usp_UserLikeGet", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Mode", 1);
                            cmd.Parameters.AddWithValue("@PostId", obj.PostId);
                            cmd.Parameters.AddWithValue("@PostUserName", obj.Username);
                            cmd.Parameters.AddWithValue("@LikeId", obj.Id);
                            cmd.Parameters.AddWithValue("@LikeUserName", obj.HdnUsername);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    IsLiked = reader.GetInt32(0) == 1;
                                }
                            }
                        };
                    }

                }
                else
                {
                    Message = "Please pass the required paremeter";
                    response = false;
                    IsLiked = false;
                }

            }
            catch (Exception ex)
            {
                Message = ex.Message;
                response = false;
            }
            return Ok(new { msg = Message, res = response, IsLiked = IsLiked });

        }

        #endregion

        #region "Add Comment Section API"

        [HttpPost]
        public IActionResult AddCommentSectionAPI([FromBody] Comment objComment)
        {
            string Message = "";
            bool response = false;
            try
            {
                if (objComment.PostId != null)
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("usp_CommentGet", con);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Mode", 1);
                        cmd.Parameters.AddWithValue("@PostId", objComment.PostId);
                        cmd.Parameters.AddWithValue("@PostCommentUsername", objComment.PostCommentUsername);
                        cmd.Parameters.AddWithValue("@PostCommentId", objComment.PostCommentId);
                        cmd.Parameters.AddWithValue("@PostAddId", objComment.PostAddId);
                        cmd.Parameters.AddWithValue("@PostAddUsername", objComment.PostAddUsername);
                        cmd.Parameters.AddWithValue("@CommentInput", objComment.CommentInput);
                        cmd.ExecuteNonQuery();

                        Message = "Comment added successfully";
                        response = true;
                    }
                }
                else
                {
                    Message = "Please pass the required Paramter!";
                    response = false;
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                response = false;
            }
            return Ok(new { msg = Message, res = response });
        }

        #endregion

        #region "Comment Get Section"

        [HttpGet]
        public IActionResult GetCommentViewAPI(string Id = "")
        {

            string Message = "";
            bool response = false;

            List<Comment> lstComment = new List<Comment>();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("usp_CommentGet", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@PostId", Id);
                    cmd.Parameters.AddWithValue("@Mode", 2);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Comment obj = new Comment
                            {
                                CommentInput = Convert.ToString(reader["CommentInput"]),
                                PostId = Convert.ToInt32(reader["PostId"]),
                                PostAddId = Convert.ToInt32(reader["PostAddId"]),
                                PostAddUsername = Convert.ToString(reader["PostAddUsername"]),
                                PostCommentUsername = Convert.ToString(reader["PostCommentUsername"])
                            };


                            lstComment.Add(obj);
                        }
                    }
                    return Ok(lstComment);

                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                response = false;
            }

            return Ok(new { msg = Message, response = Response });
        }

        #endregion


    }
}