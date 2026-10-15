using System.Net;
using Microsoft.Extensions.Options;
using TalyerApp.Application.Common.Interfaces.Notifications;
using TalyerApp.Domain.Entities;
using TalyerApp.Domain.Shared.Membership;
using TalyerApp.Domain.Shared.Result;

namespace TalyerApp.Application.Features.MembershipRequests;

public sealed class MembershipEmailService : IMembershipEmailService
{
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateRenderer _templateRenderer;
    private readonly MembershipOptions _options;

    public MembershipEmailService(
        IEmailSender emailSender,
        IEmailTemplateRenderer templateRenderer,
        IOptions<MembershipOptions> options)
    {
        _emailSender = emailSender;
        _templateRenderer = templateRenderer;
        _options = options.Value;
    }

    public Task<Result> SendConfirmRequestAsync(
        MembershipRequest request,
        string plainToken,
        CancellationToken cancellationToken = default)
    {
        var confirmUrl = MembershipConfirmUrl.Build(_options.ConfirmEmailBaseUrl, plainToken);
        var body = _templateRenderer.Render(
            EmailTemplateNames.ConfirmRequest,
            new Dictionary<string, string>
            {
                [EmailTemplatePlaceholders.ConfirmUrl] = WebUtility.HtmlEncode(confirmUrl),
                [EmailTemplatePlaceholders.FirstName] = WebUtility.HtmlEncode(request.FirstName),
                [EmailTemplatePlaceholders.LastName] = WebUtility.HtmlEncode(request.LastName),
                [EmailTemplatePlaceholders.CompanyName] = WebUtility.HtmlEncode(request.CompanyName),
                [EmailTemplatePlaceholders.LifetimeHours] =
                    EmailVerificationConstants.LifetimeHours.ToString()
            });

        return _emailSender.SendAsync(
            request.Email,
            MembershipEmailSubjects.ConfirmRequest,
            body,
            isHtml: true,
            cancellationToken);
    }

    public Task<Result> SendEmailConfirmedAsync(
        MembershipRequest request,
        CancellationToken cancellationToken = default)
    {
        var body = _templateRenderer.Render(
            EmailTemplateNames.EmailConfirmed,
            new Dictionary<string, string>
            {
                [EmailTemplatePlaceholders.FirstName] = WebUtility.HtmlEncode(request.FirstName),
                [EmailTemplatePlaceholders.LastName] = WebUtility.HtmlEncode(request.LastName),
                [EmailTemplatePlaceholders.CompanyName] = WebUtility.HtmlEncode(request.CompanyName)
            });

        return _emailSender.SendAsync(
            request.Email,
            MembershipEmailSubjects.EmailConfirmed,
            body,
            isHtml: true,
            cancellationToken);
    }
}
