﻿using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using HttpMethod = Amazon.CDK.AWS.Lambda.HttpMethod;

namespace SharedConstructs;

public class AuthorizedApi : RestApi
{
   public CognitoUserPoolsAuthorizer Authorizer { get; private set; }
   
   public AuthorizedApi(
      Construct scope,
      string id,
      RestApiProps props) : base(
      scope,
      id,
      props)
   {
   }

   public AuthorizedApi WithCognito(IUserPool cognitoUserPool)
   {
      this.Authorizer = new CognitoUserPoolsAuthorizer(
         this,
         "CognitoAuthorizer",
         new CognitoUserPoolsAuthorizerProps
         {
            CognitoUserPools = new IUserPool[]
            {
               cognitoUserPool
            },
            AuthorizerName = "cognitoauthorizer",
            IdentitySource = "method.request.header.Authorization"
         });

      return this;
   }
   
   public AuthorizedApi WithEndpoint(string path, HttpMethod method, Function function)
   {
      IResource? lastResource = this.GenerateResource(path);

      lastResource?.AddMethod(
         method.ToString().ToUpper(),
         new LambdaIntegration(function),
         new MethodOptions
         {
            MethodResponses = new IMethodResponse[]
            {
               new MethodResponse { StatusCode = "200" },
               new MethodResponse { StatusCode = "400" },
               new MethodResponse { StatusCode = "500" }
            },
            AuthorizationType = AuthorizationType.COGNITO,
            Authorizer = this.Authorizer
         });

      return this;
   }
   
   public AuthorizedApi WithUnauthorisedEndpoint(string path, HttpMethod method, Function function)
   {
      IResource? lastResource = this.GenerateResource(path);

      lastResource?.AddMethod(
         method.ToString().ToUpper(),
         new LambdaIntegration(function),
         new MethodOptions
         {
            MethodResponses = new IMethodResponse[]
            {
               new MethodResponse { StatusCode = "200" },
               new MethodResponse { StatusCode = "400" },
               new MethodResponse { StatusCode = "500" }
            }
         });

      return this;
   }

   private IResource GenerateResource(string path)
   {
      IResource? lastResource = null;
      
      foreach (var pathSegment in path.Split('/'))
      {
         var sanitisedPathSegment = pathSegment.Replace(
            "/",
            "");

         if (string.IsNullOrEmpty(sanitisedPathSegment))
         {
            continue;
         }

         if (lastResource == null)
         {
            lastResource = this.Root.GetResource(sanitisedPathSegment) ?? this.Root.AddResource(sanitisedPathSegment);
            continue;
         }

         lastResource = lastResource.GetResource(sanitisedPathSegment) ??
                        lastResource.AddResource(sanitisedPathSegment);
      }

      return lastResource;
   }
}