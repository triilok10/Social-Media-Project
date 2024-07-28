﻿using Microsoft.AspNetCore.Http;
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

        public AccountAPI(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CustomConnection");
        }

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
        [HttpGet]
        public IActionResult GetUserHomeDetails(int Id)
        {
            try
            {
                MediaPost objAccount = new MediaPost();
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("usp_GetUserDetails", con);
                    con.Open();
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", Id);
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            objAccount.Fullname = Convert.ToString(rdr["FullName"]);
                            objAccount.ProfilePhotoPath = Convert.ToString(rdr["ProfilePhotoPath"]);
                            objAccount.Bio = Convert.ToString(rdr["ProfileBio"]);
                            objAccount.DateOfBirth = Convert.ToDateTime(rdr["DateOfBirth"]);
                        }
                    }
                }
                return Ok(objAccount);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }
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
                        cmd.Parameters.AddWithValue("@Id",pMediaPost.Id);
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
    }
}
