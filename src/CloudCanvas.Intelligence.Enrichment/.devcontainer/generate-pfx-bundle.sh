
####### IMPORTANT: This script is intended to be run inside the devcontainer, not on the host machine.
#### What it does: Generates a PFX bundle for the Cosmos DB emulator certificate.
## Should only be run once, when the devcontainer is first created. 
# The generated PFX bundle will be used to configure the Cosmos DB emulator to use a self-signed certificate for HTTPS connections.
# It allows me to determine the hostnames included in the certificate's Subject Alternative Name (SAN) extension, which is important for local development and testing.
# ALL OF THIS IS ONLY FOR DEVELOPMENT PURPOSES. DO NOT USE IN PRODUCTION.

openssl_dir=".devcontainer/openssl"
bundle=".devcontainer/cosmos-certificate-bundle.pem"
url="https://cosmosdb:1234/_explorer/emulator.pem"
cert_target_dir="/usr/local/share/ca-certificates/"
emulator_key="$openssl_dir/cosmos-emulator.key"
openssl_cfg="$openssl_dir/cosmos-openssl.cnf"
emulator_cert="$openssl_dir/cosmos-emulator.crt"
emulator_pfx="$openssl_dir/cosmos-emulator.pfx"
passout="root"

# in case the directory does not exist, create it
# this is where the generated certificate and key will be stored
mkdir -p $openssl_dir

# generate a new self-signed certificate and key for the Cosmos emulator
# using OpenSSL with a custom configuration file
openssl req \
  -x509 -newkey rsa:2048 -nodes -days 365 \
  -keyout $emulator_key \
  -out $emulator_cert \
  -config $openssl_cfg

# export the certificate and key to a PFX file
openssl pkcs12 -export \
  -inkey $emulator_key \
  -in $emulator_cert \
  -out $emulator_pfx \
  -passout pass:$passout

# verify the generated certificate
openssl x509 -in $emulator_cert -noout -ext subjectAltName
# copy the generated certificate to the system trust store
cp $openssl_dir/*.crt $cert_target_dir
# finally, update the system trust store to include the new certificate
update-ca-certificates