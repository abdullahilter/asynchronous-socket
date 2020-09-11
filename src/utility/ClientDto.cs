using System;

namespace utility
{
    /// <summary>
    /// 
    /// </summary>
    public class ClientDto
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime LastRequestDateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsWarned { get; set; }

        #endregion

        #region Constructors

        public ClientDto(Guid id, DateTime lastRequestDateTime)
        {
            Id = id;
            LastRequestDateTime = lastRequestDateTime;
        }

        #endregion
    }
}