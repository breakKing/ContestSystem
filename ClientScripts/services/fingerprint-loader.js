import FingerprintJS from '@fingerprintjs/fingerprintjs'

const generateFingerprint = async () => {
    const fpPromise = FingerprintJS.load()

    const fp = await fpPromise
    const result = await fp.get()

    return result.visitorId
}

export default generateFingerprint;