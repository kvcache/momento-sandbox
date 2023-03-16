import {fromNodeProviderChain} from '@aws-sdk/credential-providers';
import {GetCallerIdentityCommand, STSClient} from '@aws-sdk/client-sts';
import {SignatureV4} from '@aws-sdk/signature-v4';
import {Sha256} from '@aws-crypto/sha256-js';
import {buildQueryString} from '@aws-sdk/querystring-builder';

// See: https://github.com/aws/aws-sdk-js-v3/tree/b190dcc4f83f629e947073e8607325a2862c5b22/clients/browser/client-sts-browser
// and: https://dev.to/aws-builders/signing-requests-with-aws-sdk-in-lambda-functions-476

async function main() {
  const credsProvider = fromNodeProviderChain();
  const creds = await credsProvider();
  const stsClient = new STSClient({
    credentials: creds,
    // logger: console,
  });
  const request = new GetCallerIdentityCommand({});
  const identity = await stsClient.send(request);
  // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
  console.log(`IDENTITY IS: ${identity.Arn!}`);
  const sigv4 = new SignatureV4({
    service: 'sts',
    region: 'us-west-2',
    credentials: creds,
    sha256: Sha256,
  });

  const url = new URL('https://sts.us-west-2.amazonaws.com');

  const signed = await sigv4.presign({
    method: 'GET',
    hostname: url.host,
    protocol: url.protocol,
    path: url.pathname,
    query: {
      Action: 'GetCallerIdentity',
      Version: '2011-06-15',
    },
    headers: {
      'Content-Type': 'application/json',
      host: url.hostname,
    },
  });

  console.log(`Here is a temporary curl command that uses a pre-signed URL to get your AWS caller identity:
  
curl -H "Content-type: application/json" -H "host: ${url.hostname}" "${
    url.protocol
    // eslint-disable-next-line @typescript-eslint/no-non-null-assertion
  }//${url.host}${url.pathname}?${buildQueryString(signed.query!)}"  
  
  
  `);
}

main().catch(e => {
  throw e;
});
