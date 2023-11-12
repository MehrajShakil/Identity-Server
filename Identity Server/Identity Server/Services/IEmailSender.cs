﻿using Identity_Server.Entities;

namespace Identity_Server.Services;

public interface IEmailSender
{
    Task<bool> SendEmailAsync(MailData mailData);
}