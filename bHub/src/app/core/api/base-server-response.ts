export class BaseServerResponse<ResponseData> {
    dateTimeStamp?: Date;
    isSuccessful?: boolean;
    resultData?: ResponseData;
}