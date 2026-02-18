using System;
using DesignPatternChallenge.Interfaces;
using DesignPatternChallenge.Legacy;
using DesignPatternChallenge.Models;

namespace DesignPatternChallenge.Adapters;

/// <summary>
/// Adapter que permite o uso do LegacyPaymentSystem através da interface IPaymentProcessor.
/// Encapsula todas as conversões de tipos e assinaturas entre as interfaces incompatíveis.
/// </summary>
public class LegacyPaymentAdapter : IPaymentProcessor
{
    private readonly LegacyPaymentSystem _legacySystem;

    public LegacyPaymentAdapter(LegacyPaymentSystem legacySystem)
    {
        _legacySystem = legacySystem;
    }

    public PaymentResult ProcessPayment(PaymentRequest request)
    {
        // Converte os dados da interface moderna para o formato legado
        var cvvCode = int.Parse(request.Cvv);
        var expMonth = request.ExpirationDate.Month;
        var expYear = request.ExpirationDate.Year;
        var amountInCents = (double)(request.Amount * 100);

        var legacyResponse = _legacySystem.AuthorizeTransaction(
            request.CreditCardNumber,
            cvvCode,
            expMonth,
            expYear,
            amountInCents,
            request.CustomerEmail
        );

        // Converte a resposta legada para o formato moderno
        return new PaymentResult
        {
            Success = legacyResponse.ResponseCode == "00",
            TransactionId = legacyResponse.TransactionRef,
            Message = legacyResponse.ResponseMessage
        };
    }

    public bool RefundPayment(string transactionId, decimal amount)
    {
        var amountInCents = (double)(amount * 100);
        return _legacySystem.ReverseTransaction(transactionId, amountInCents);
    }

    public PaymentStatus CheckStatus(string transactionId)
    {
        var legacyStatus = _legacySystem.QueryTransactionStatus(transactionId);

        return legacyStatus switch
        {
            "APPROVED" => PaymentStatus.Approved,
            "DECLINED" => PaymentStatus.Declined,
            "REFUNDED" => PaymentStatus.Refunded,
            _ => PaymentStatus.Pending
        };
    }
}
