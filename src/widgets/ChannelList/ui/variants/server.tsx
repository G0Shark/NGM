import { Suspense, useEffect, useState } from "react"
import { ServerChannelListProps } from "../../model/variants"
import { graphql, GraphQLSchema, GraphQLObjectType, GraphQLString } from 'graphql';

interface IChat {
  id: number,
  serverId: number,
  rolesId: string,
  name: string,
  description: string,
  type: string
};

export default function ServerChannelList({ serverId }: ServerChannelListProps) {
  const { 0: channels, 1: setChannels } = useState<IChat[]>([]);
  const { 0: isLoading, 1: setIsLoading } = useState(true);
  useEffect(() => {
    const fetchChannels = async () => {

    };
    fetchChannels();
  }, []);
  return (<Suspense fallback={<div>Loading the channels...</div>}>

  </Suspense>)
}