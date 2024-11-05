module BackEnd4IdleStrategyFS.Test.Docs.Grokking.ReaderMonad
// https://dev.to/choc13/grokking-traversable-bla

// 领域模型
type CreditCard =
    { Number: string
      Expiry: string
      Cvv: string }

type EmailAddress = EmailAddress of string
type UserId = UserId of string
type PaymentId = PaymentId of string
type EmailId = EmailId of string
type EmailBody = EmailBody of string

type User =
    { Id: UserId
      CreditCard: CreditCard
      EmailAddress: EmailAddress }
// 模块
type ISqlConnection =
    abstract Query: string -> User (* 'T *)

module Database =
    let getUser (UserId id) (connection: ISqlConnection) : User =
        connection.Query($"SELECT * FROM User AS u  WHERE u.Id = {id}")

type IPaymentClient =
    abstract Charge: CreditCard -> float -> PaymentId

module PaymentProvider =
    let chargeCard (card: CreditCard) amount (client: IPaymentClient) = client.Charge card amount

type IEmailClient =
    abstract SendMail: EmailAddress -> EmailBody -> EmailId

module Email =
    let sendMail (address: EmailAddress) (body: EmailBody) (client: IEmailClient) = client.SendMail address body

let injectSqlConnection f valueFromConnection =
    fun connection ->
        let value = valueFromConnection connection
        f value

let chargeUser userId amount =
    Database.getUser userId
    |> injectSqlConnection (fun user -> PaymentProvider.chargeCard user.CreditCard amount)

// 一个小小的谎言
let inject f valueThatNeedsDep =
    fun deps ->
        let value = valueThatNeedsDep deps
        f value deps

type InjectorBuilder() =
    member _.Return(x) = fun _ -> x
    member _.Bind(x, f) = inject f x
    member _.Zero() = fun _ -> ()
    member _.ReturnFrom x = x

let injector = InjectorBuilder()


let chargeUser' userId amount =
    injector {
        let! user = Database.getUser userId
        let! paymentId = PaymentProvider.chargeCard user.CreditCard amount
        let email = EmailBody $"Your payment id is {paymentId}"
        return! Email.sendMail user.EmailAddress email
    }

type IDeps =
    inherit IPaymentClient
    inherit ISqlConnection
    inherit IEmailClient

let deps =
    { new IDeps with
        member _.Charge card amount =
            // create PaymentClient and call it
            PaymentId ""

        member _.SendMail address body =
            // create SMTP client and call it
            EmailId ""

        member _.Query x =
            // create sql connection and invoke it
            { Id = UserId "1"
              CreditCard = { Number = ""; Expiry = ""; Cvv = "" }
              EmailAddress = EmailAddress "" } }

let paymentId = chargeUser' (UserId "1") 2.50 deps

// 附录
type Reader<'env, 'a> = Reader of ('env -> 'a)

module Reader =
    let run (Reader x) = x
    let map f reader = Reader((run reader) >> f)

    let bind f reader =
        Reader(fun env ->
            let a = run reader env
            let newReader = f a
            run newReader env)

    let ask = Reader id

type ReaderBuilder() =
    member _.Return(x) = Reader(fun _ -> x)
    member _.Bind(x, f) = Reader.bind f x
    member _.Zero() = Reader(fun _ -> ())
    member _.ReturnFrom x = x

let reader = ReaderBuilder()

let chargeUser'' userId amount =
    reader {
        let! (sqlConnection: #ISqlConnection) = Reader.ask
        let! (paymentClient: #IPaymentClient) = Reader.ask
        let! (emailClient: #IEmailClient) = Reader.ask
        let user = Database.getUser userId sqlConnection
        let paymentId = PaymentProvider.chargeCard user.CreditCard amount paymentClient

        let email = EmailBody $"Your payment id is {paymentId}"

        return Email.sendMail user.EmailAddress email emailClient
    }

let paymentId'' = Reader.run (chargeUser'' (UserId "1") 2.50) deps
