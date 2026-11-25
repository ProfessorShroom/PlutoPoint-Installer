import getpass
import hashlib
import secrets

ITERATIONS = 600_000


def encode(sha256_hex):
    salt = secrets.token_bytes(16)
    key = hashlib.pbkdf2_hmac("sha512", sha256_hex.encode("ascii"), salt, ITERATIONS, 32)
    return f"pbkdf2-sha512${ITERATIONS}${salt.hex()}${key.hex()}"


def hash_secret(secret):
    return encode(hashlib.sha256(secret.encode("utf-8")).hexdigest())


if __name__ == "__main__":
    secret = getpass.getpass("Password or IP address: ")
    if not secret or secret != getpass.getpass("Again: "):
        raise SystemExit("Empty or didn't match.")
    print(hash_secret(secret))
