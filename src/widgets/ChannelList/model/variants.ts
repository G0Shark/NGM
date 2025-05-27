// widgets/ChannelList/model/variants.ts
export type ChannelListVariants = "server" | "user";

export interface ServerChannelListProps {
  serverId: number;
}

export interface UserChannelListProps {

}

export type ChannelListVariantPropsMap = {
  server: ServerChannelListProps;
  user: UserChannelListProps;
};
