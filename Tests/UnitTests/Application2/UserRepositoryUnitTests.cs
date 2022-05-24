using NUnit.Framework;
using System;
using System.Threading;
using Application2.Models;
using Application2.Services;

namespace Tests.UnitTests.Application2
{
    public class UserRepositoryUnitTests
    {

        private UserRepository _userRepository;

        private int _id;
        private User AddUser()
        {
            User user = new(
                Interlocked.Increment(ref _id).ToString(), 
                "Test");

            _userRepository.Add(user);

            return user;
        }


        [SetUp]
        public void Setup()
        {
            _userRepository = new UserRepository();
        }

        #region ADD
        [Test]
        public void Add_NullUser_ThrowsException()
        {
            User? user = null;

            Assert.Throws<ArgumentNullException>(() => _userRepository.Add(user));
        }

        public void Add_DuplicateUser_ThrowsException()
        {
            var duplicateId = "1";

            User user = new(duplicateId, "Test");
            User duplicate = new(duplicateId, "Second");

            _userRepository.Add(user);

            Assert.Throws<InvalidOperationException>(() => _userRepository.Add(duplicate));
        }

        [Test]
        public void Add_NullOrWhiteSpaceId_ThrowsException()
        {
            User nullId =new(null, "Test");
            Assert.Throws<ArgumentException>(() => _userRepository.Add(nullId));

            User whiteSpaceId = new(string.Empty, "Test");
            Assert.Throws<ArgumentException>(()=>_userRepository.Add(whiteSpaceId));
        }


        [Test]
        public void Add_NullName_ThrowsException()
        {
            User nullName = new("1", null);
            Assert.Throws<ArgumentException>(() => _userRepository.Add(nullName));

            User whiteSpaceName = new("2", string.Empty);
            Assert.Throws<ArgumentException>(() => _userRepository.Add(whiteSpaceName));
        }
        #endregion

        #region DELETE


        [Test]
        public void Delete_NullOrWhiteSpaceId_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _userRepository.Delete(null));
            Assert.Throws<ArgumentException>(() => _userRepository.Delete(string.Empty));
            Assert.Throws<ArgumentException>(() => _userRepository.Delete(" "));
        }

        [Test]
        public void Delete_UserNotFound_ReturnsNull()
        {
            var deletedUser = _userRepository.Delete("id_that_doesn't_exist");

            Assert.IsNull(deletedUser);
        }

        [Test]
        public void Delete_UserExists_DeletesAndReturnsUser()
        {
            var addedUser = AddUser();
            var returnedUser = _userRepository.Delete(addedUser.Id);

            Assert.IsNotNull(returnedUser);
            Assert.AreSame(addedUser, returnedUser);

            // make sure the user is not present anymore.
            Assert.IsNull(_userRepository.Get(addedUser.Id));
        }
        #endregion


        #region GET
        public void Get_UserNotFound_ReturnsNull()
        {
            var user = _userRepository.Get("id_that_doesn't_exist");
            Assert.IsNull(user);
        }

        [Test]
        public void Get_NullOrWhiteSpaceId_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _userRepository.Get(null));
            Assert.Throws<ArgumentException>(() => _userRepository.Get(string.Empty));
            Assert.Throws<ArgumentException>(() => _userRepository.Get(" "));
        }

        [Test]
        public void Get_UserExists_ReturnsUser()
        {
            var addedUser = AddUser();
            var fetchedUser = _userRepository.Get(addedUser.Id);

            Assert.AreSame(addedUser, fetchedUser);
        }

        [Test]
        public void GetAll_ReturnsUsers()
        {
            var user1 = AddUser();
            var user2 = AddUser();
            var user3 = AddUser();

            var users = _userRepository.Get();

            CollectionAssert.AllItemsAreNotNull(users);
            CollectionAssert.AllItemsAreUnique(users);

            CollectionAssert.IsNotEmpty(users);

            CollectionAssert.Contains(users, user1);
            CollectionAssert.Contains(users, user2);
            CollectionAssert.Contains(users, user3);

            var deletedUser1 = _userRepository.Delete(user1.Id);
            Assert.IsNotNull(deletedUser1);
            Assert.AreSame(deletedUser1, user1);

            var deletedUser2 = _userRepository.Delete(user2.Id);
            Assert.IsNotNull(deletedUser2);
            Assert.AreSame(deletedUser2, user2);

            var deletedUser3 = _userRepository.Delete(user3.Id);
            Assert.IsNotNull(deletedUser3);
            Assert.AreSame(deletedUser3, user3);

            var usersAfterDelete = _userRepository.Get();

            CollectionAssert.IsEmpty(usersAfterDelete);
        }
        #endregion
    }
}
