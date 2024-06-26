﻿using Checkr.Extensions;
using Checkr.Services.Abstract;
using Checkr.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Checkr.Controllers
{
    [Authorize]
    public class BoardsController(
        IBoardService boardService,
        IUserService userService) : Controller
    {
        private readonly IBoardService _boardService = boardService;
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = await _userService.GetUserIdAsync(User);

            var boards = await _boardService.GetAllBoardsForUserAsync(userId);

            return View(boards);
        }

        [HttpGet]
        [Authorize(Policy = "IsUserPolicy")]
        public async Task<IActionResult> Details(int id)
        {
            var board = await _boardService.GetBoardByIdAsync(id);

            return View(board);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = await _userService.GetUserIdAsync(User);

            return View(new BoardDto { OwnerId = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BoardDto boardDto)
        {
            if (!ModelState.IsValid)
            {
                return View(boardDto);
            }

            await _boardService.CreateBoardAsync(boardDto);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = "IsUserPolicy")]
        public async Task<IActionResult> Update(int id)
        {
            var board = await _boardService.GetBoardByIdAsync(id);

            var boardDto = board.ToBoardDto();

            return View(boardDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, BoardDto boardDto)
        {
            if (!ModelState.IsValid)
            {
                return View(boardDto);
            }

            await _boardService.UpdateBoardAsync(id, boardDto);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Policy = "IsOwnerPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _boardService.DeleteBoardAsync(id);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = "IsUserPolicy")]
        public async Task<IActionResult> Users(int id)
        {
            var board = await _boardService.GetBoardByIdAsync(id);

            return View(board);
        }

        [HttpPost]
        [Authorize(Policy = "IsOwnerPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUserFromBoard(int id, string userId)
        {
            await _boardService.RemoveUserFromBoardAsync(id, userId);

            return RedirectToAction(nameof(Users), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveBoard(int id)
        {
            var board = await _boardService.GetBoardByIdAsync(id);

            var userId = await _userService.GetUserIdAsync(User);

            if (board.OwnerId == userId)
            {
                return Forbid();
            }

            await _boardService.RemoveUserFromBoardAsync(id, userId);

            return RedirectToAction(nameof(Index));
        }
    }
}
