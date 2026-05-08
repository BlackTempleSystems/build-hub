export class BaseServerResponse<ResponseData> {
    dateTimeStamp?: Date;
    isSuccessful?: boolean;
    response?: ResponseData;
}