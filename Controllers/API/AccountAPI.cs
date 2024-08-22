using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Project.Models;
using System.Data;
using System.Data.SqlClient;

namespace Social_Media_Project.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountAPI : ControllerBase
    {
        private readonly string _connectionString;
        private readonly EncryptionHelper _encryptionHelper;

        public AccountAPI(IConfiguration configuration, EncryptionHelper encryptionHelper)
        {
            _connectionString = configuration.GetConnectionString("CustomConnection");
            _encryptionHelper = encryptionHelper;
        }


        #region "Test API"
        [HttpGet]
        public IActionResult TestAPI()
        {
            string plainTextMessage = "Your Test Message here.";
            string encryptedMessage = _encryptionHelper.Encrypt(plainTextMessage);
            return Ok(new { encryptedMsg = encryptedMessage, plain = plainTextMessage });
        }

        [HttpPost]
        public IActionResult DecryptMessage([FromBody] Decrypted request)
        {
            if (string.IsNullOrEmpty(request.EncryptedMsg))
            {
                return BadRequest(new { error = "EncryptedMsg field is required." });
            }

            try
            {
                string decryptedMessage = _encryptionHelper.Decrypt(request.EncryptedMsg);
                return Ok(new { decryptedMessage });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Decryption failed", message = ex.Message });
            }
        }

        #endregion

        #region "SignUp Get/Post"

        [HttpPost]
        public IActionResult Signup([FromBody] Account pAccount)
        {
            string Message = "";
            string AccountId = "";
            try
            {
                if (pAccount.Id == 0)
                {
                    if (pAccount == null)
                    {
                        Message = "Please fill the required inputs.";
                        return Ok(new { msg = Message });
                    }

                    if (pAccount.Password != pAccount.ConfirmPassword)
                    {
                        Message = "Password and Confirm-Password didn't match!";
                        return Ok(new { msg = Message });
                    }

                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand("usp_SignUp", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Mode", 1);
                        cmd.Parameters.AddWithValue("@Mobile", pAccount.Phone);
                        cmd.Parameters.AddWithValue("@Email", pAccount.Email);
                        cmd.Parameters.AddWithValue("@Fullname", pAccount.Fullname);
                        cmd.Parameters.AddWithValue("@NewPassword", pAccount.Password);
                        cmd.Parameters.AddWithValue("@ConfirmPassword", pAccount.ConfirmPassword);
                        cmd.Parameters.AddWithValue("@Username", pAccount.Username);
                        cmd.Parameters.AddWithValue("@FCMToken", pAccount.FcmToken);

                        SqlParameter outputMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 200);
                        outputMessage.Direction = ParameterDirection.Output;
                        SqlParameter signupId = new SqlParameter("@AccountId", SqlDbType.NVarChar, 20);
                        signupId.Direction = ParameterDirection.Output;

                        cmd.Parameters.Add(outputMessage);
                        cmd.Parameters.Add(signupId);
                        cmd.ExecuteNonQuery();
                        Message = cmd.Parameters["@Message"].Value.ToString();
                        AccountId = cmd.Parameters["@AccountId"].Value.ToString();
                    }

                    return Ok(new { msg = Message, id = AccountId });
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("usp_SignUp", con);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Mode", 2);
                        cmd.Parameters.AddWithValue("@Id", pAccount.Id);
                        cmd.Parameters.AddWithValue("@ProfilePhotoPath", pAccount.ProfilePhotoPath);
                        cmd.Parameters.AddWithValue("@Bio", pAccount.Bio);
                        cmd.Parameters.AddWithValue("@DateOfBirth", pAccount.DateOfBirth);
                        SqlParameter outputMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 200);
                        outputMessage.Direction = ParameterDirection.Output;
                        SqlParameter signupId = new SqlParameter("@AccountId", SqlDbType.NVarChar, 20);
                        signupId.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputMessage);
                        cmd.Parameters.Add(signupId);
                        cmd.ExecuteNonQuery();
                        Message = cmd.Parameters["@Message"].Value.ToString();
                        AccountId = cmd.Parameters["@AccountId"].Value.ToString();
                    }
                    return Ok(new { msg = Message, id = AccountId });
                }

            }
            catch (SqlException ex)
            {
                Message = ex.Message;
                return Ok(new { msg = Message });
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new { msg = Message });
            }
        }

        #endregion

        #region "Get User Home Details"

        #region "GetUserBioDetails"
        [HttpGet]
        public IActionResult GetUserBioDetails(int Id)
        {
            string Message = "";
            try
            {
                MediaPost objAccount = new MediaPost();
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("usp_GetUserDetails", con);
                    con.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", 1);
                    cmd.Parameters.AddWithValue("@Id", Id);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            objAccount.Fullname = Convert.ToString(rdr["FullName"]);
                            objAccount.ProfilePhotoPath = Convert.ToString(rdr["ProfilePhotoPath"]);
                            objAccount.Bio = Convert.ToString(rdr["ProfileBio"]);
                            objAccount.DateOfBirth = Convert.ToDateTime(rdr["DateOfBirth"]);
                            objAccount.Username = Convert.ToString(rdr["Username"]);
                            objAccount.Id = Convert.ToInt32(rdr["Id"]);
                        }
                    }
                }
                return Ok(objAccount);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new { msg = Message });
            }

        }

        #endregion

        #region "GetFollowingData"
        [HttpGet]
        public IActionResult GetFollowingData(string Id)
        {
            string Message = "";
            try
            {
                if (Id != null)
                {
                    MediaPost mediaPost = new MediaPost();
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("usp_FollowGet", con);
                        con.Open();
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Mode", 3);
                        cmd.Parameters.AddWithValue("@FollowingId", Id);

                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                mediaPost.Follower = Convert.ToString(rdr["FollowCount"]);
                                mediaPost.Following = Convert.ToString(rdr["FollowingCount"]);
                                mediaPost.PostCount = Convert.ToString(rdr["PostCount"]);
                            }
                        }
                        return Ok(mediaPost);
                    }
                }
                else
                {
                    Message = "Please select the user Correctly";
                    return Ok(new { msg = Message });
                }


            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new { msg = Message });
            }


        }
        #endregion


        #region "GetUserPostDetails"
        [HttpGet]
        public IActionResult GetUserPostDetails(int Id)
        {
            string Message = "";
            try
            {
                List<MediaPost> ltrMediaPost = new List<MediaPost>();
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("usp_GetUserDetails", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Mode", 2);
                    cmd.Parameters.AddWithValue("@Id", Id);

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            MediaPost objAccount = new MediaPost
                            {
                                PhotoPath = Convert.ToString(rdr["PhotoPath"]),
                                PostCaption = Convert.ToString(rdr["PostCaption"])
                            };
                            ltrMediaPost.Add(objAccount);
                        }
                    }
                }
                return Ok(ltrMediaPost);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return StatusCode(500, new { msg = Message });
            }
        }
        #endregion

        #endregion


        #region "Login Verify"
        [HttpPost]
        public IActionResult LoginVerify([FromBody] Account pAccount)
        {
            string Message = "";
            string UserIdSQL = "";
            string ErrorMessageSQL = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(pAccount.Username) && !string.IsNullOrWhiteSpace(pAccount.Password))
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand("usp_loginVerify", con);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Username", pAccount.Username);
                        cmd.Parameters.AddWithValue("@Password", pAccount.Password);
                        SqlParameter UserId = new SqlParameter("@UserId", SqlDbType.Int, 5);
                        UserId.Direction = ParameterDirection.Output;
                        SqlParameter ErrorMessage = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 100);
                        ErrorMessage.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(UserId);
                        cmd.Parameters.Add(ErrorMessage);
                        cmd.ExecuteNonQuery();
                        UserIdSQL = cmd.Parameters["@UserId"].Value.ToString();
                        ErrorMessageSQL = cmd.Parameters["@ErrorMessage"].Value.ToString();
                    }
                    return Ok(new { id = UserIdSQL, errmsg = ErrorMessageSQL });
                }
                else
                {
                    Message = "Please pass the required parameters";
                    return Ok(new { msg = Message });
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new { msg = Message });
            }
        }
        #endregion

        #region "Create Post"
        [HttpPost]
        public IActionResult CreatePost([FromBody] MediaPost pMediaPost)
        {
            string Message = "";
            try
            {
                if (pMediaPost.Id != null && pMediaPost.PhotoPath != null)
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("usp_UserPost", con);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        con.Open();
                        cmd.Parameters.AddWithValue("@Id", pMediaPost.Id);
                        cmd.Parameters.AddWithValue("@Mode", 1);
                        cmd.Parameters.AddWithValue("@Photopath", pMediaPost.PhotoPath);
                        cmd.Parameters.AddWithValue("@PostCaption", pMediaPost.PostCaption);
                        cmd.ExecuteNonQuery();
                    }
                    Message = "Post added successfully.";
                    return Ok(new { msg = Message });
                }
                else
                {
                    Message = "Please pass the required parameters.";
                    return Ok(new { msg = Message });
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new { msg = Message });
            }
        }
        #endregion


        #region "Search Profile"
        [HttpGet]
        public IActionResult SearchProfile(string SearchName = "")
        {
            try
            {
                List<MediaPost> lstPost = new List<MediaPost>();

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("usp_UserPost", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Mode", 2);
                    cmd.Parameters.AddWithValue("@Search", SearchName ?? string.Empty);

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MediaPost obj = new MediaPost
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Fullname = Convert.ToString(reader["FullName"]),
                                Username = Convert.ToString(reader["Username"]),
                                ProfilePhotoPath = Convert.ToString(reader["ProfilePhotoPath"]),
                                Bio = Convert.ToString(reader["ProfileBio"])
                            };
                            lstPost.Add(obj);
                        }
                    }
                    return Ok(lstPost);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    msg = ex.Message
                });
            }
        }
        #endregion


        #region "Feed Data"
        [HttpGet]
        public IActionResult FeedData()
        {
            string Message = "";
            List<MediaPost> lstPost = new List<MediaPost>();
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("usp_UserPost", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Mode", 3);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {

                        while (rdr.Read())
                        {
                            MediaPost obj = new MediaPost
                            {
                                Fullname = Convert.ToString(rdr["FullName"]),
                                Username = Convert.ToString(rdr["Username"]),
                                ProfilePhotoPath = Convert.ToString(rdr["ProfilePhotoPath"]),
                                PhotoPath = Convert.ToString(rdr["PhotoPath"]),
                                PostCaption = Convert.ToString(rdr["PostCaption"]),
                                Id = Convert.ToInt32(rdr["UserId"]),
                                UserId = Convert.ToInt32(rdr["Id"])
                            };
                            lstPost.Add(obj);
                        }
                    }
                    return Ok(lstPost);
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new { msg = Message });
            }

        }
        #endregion

        //#region "Search User Profile"
        //[HttpGet]
        //public IActionResult SearchUserHomePage(string Id = "")
        //{
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(_connectionString))
        //        {
        //            SqlCommand cmd = new SqlCommand("", con);
        //            cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //            con.Open();
        //            cmd.Parameters.AddWithValue("@Id", Id);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return Ok();
        //}
        //#endregion


        #region "UpdateUserProfile"
        [HttpGet]
        public IActionResult UpdateUserProfile(string Id = "")
        {
            string Message = "";
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("usp_GetUserDetails", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.Parameters.AddWithValue("@Mode", 1);

                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            MediaPost obj = new MediaPost
                            {
                                Fullname = Convert.ToString(rdr["Fullname"]),
                                Bio = Convert.IsDBNull(rdr["ProfileBio"]) ? null : Convert.ToString(rdr["ProfileBio"]),
                                DateOfBirth = Convert.IsDBNull(rdr["DateOfBirth"]) ? (DateTime?)null : Convert.ToDateTime(rdr["DateOfBirth"]),
                                ProfilePhotoPath = Convert.IsDBNull(rdr["ProfilePhotoPath"]) ? null : Convert.ToString(rdr["ProfilePhotoPath"]),
                                Mobile = Convert.ToString(rdr["Mobile"]),
                                Email = Convert.ToString(rdr["Email"]),
                                Username = Convert.ToString(rdr["Username"])
                            };
                            return Ok(obj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateProfilePost([FromBody] MediaPost objMediaPost)
        {
            string Message = "";
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("UpdateProfile", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", objMediaPost.Id);
                    cmd.Parameters.AddWithValue("@ProfilePhotoPath", objMediaPost.ProfilePhotoPath);
                    cmd.Parameters.AddWithValue("@Bio", objMediaPost.Bio);
                    cmd.Parameters.AddWithValue("@Mobile", objMediaPost.Mobile);
                    cmd.Parameters.AddWithValue("@Fullname", objMediaPost.Fullname);
                    cmd.Parameters.AddWithValue("@DateOfBirth", objMediaPost.DateOfBirth);
                    SqlParameter message = new SqlParameter("@Message", SqlDbType.VarChar, 200);
                    message.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(message);

                    cmd.ExecuteNonQuery();
                    Message = cmd.Parameters["@Message"].Value.ToString();

                    return Ok(new { msg = Message });
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return Ok(new { errmsg = Message });
            }

        }
        #endregion

        #region "Follow Up"

        [HttpPost]
        public IActionResult CheckFollowStatus([FromBody] MediaPost obj)
        {
            bool isFollowing = false;

            try
            {

                if (obj.Id != null)
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        SqlCommand cmd = new SqlCommand("usp_FollowGet", con);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        con.Open();


                        cmd.Parameters.AddWithValue("@Mode", 1);
                        cmd.Parameters.AddWithValue("@FollowId", obj.hdnId);
                        cmd.Parameters.AddWithValue("@FollowUserName", obj.Username);
                        cmd.Parameters.AddWithValue("@UserId", obj.Id);
                        cmd.Parameters.AddWithValue("@Username", obj.HdnUsername);


                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            isFollowing = reader.GetInt32(0) == 1;
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new { error = ex.Message });
            }

            return Ok(new { isFollowing });
        }



        [HttpPost]
        public IActionResult UserFollowup([FromBody] MediaPost obj)
        {
            string Message = "";
            bool isFollowing = false;

            try
            {
                if (obj.Id != null && obj.hdnId != null && obj.Username != null)
                {
                    using (SqlConnection con = new SqlConnection(_connectionString))
                    {
                        con.Open();


                        using (SqlCommand cmd = new SqlCommand("usp_FollowGet", con))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@Mode", 1);
                            cmd.Parameters.AddWithValue("@FollowId", obj.hdnId);
                            cmd.Parameters.AddWithValue("@FollowUserName", obj.Username);
                            cmd.Parameters.AddWithValue("@UserId", obj.Id);
                            cmd.Parameters.AddWithValue("@Username", obj.HdnUsername);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    isFollowing = reader.GetInt32(0) == 1;
                                }
                            }
                        }


                        if (isFollowing)
                        {
                            using (SqlCommand cmd = new SqlCommand("usp_FollowGet", con))
                            {
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@Mode", 4);
                                cmd.Parameters.AddWithValue("@FollowId", obj.hdnId);
                                cmd.Parameters.AddWithValue("@UserId", obj.Id);

                                cmd.ExecuteNonQuery();
                                Message = "Unfollowed Successfully";
                                isFollowing = false;
                            }
                        }
                        else
                        {
                            using (SqlCommand cmd = new SqlCommand("usp_FollowGet", con))
                            {
                                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@Mode", 2);
                                cmd.Parameters.AddWithValue("@FollowId", obj.hdnId);
                                cmd.Parameters.AddWithValue("@FollowUserName", obj.Username);
                                cmd.Parameters.AddWithValue("@UserId", obj.Id);
                                cmd.Parameters.AddWithValue("@Username", obj.HdnUsername);

                                cmd.ExecuteNonQuery();
                                Message = "Followed successfully";
                                isFollowing = true;
                            }
                        }
                    }
                }
                else
                {
                    Message = "Please select the correct user to follow";
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }

            return Ok(new { msg = Message, isFollowing = isFollowing });
        }



        #endregion

    }
}
